using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CandyHandyman.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly INotificationHub _notificationHub;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        AppDbContext context,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        INotificationHub notificationHub,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _notificationHub = notificationHub;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string content, NotificationType type, Guid? relatedId = null, string? relatedType = null)
    {
        var setting = await _context.NotificationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Type == type);

        if (setting != null && !setting.Enabled)
            return;

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Content = content,
            Type = type,
            RelatedId = relatedId,
            RelatedType = relatedType
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        await _notificationHub.SendToUserAsync(userId, "ReceiveNotification", new
        {
            notification.Id,
            notification.Title,
            notification.Content,
            Type = notification.Type.ToString(),
            notification.RelatedId,
            notification.RelatedType,
            notification.CreatedAt
        });

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                var fcmToken = await _context.UserFcmTokens.FirstOrDefaultAsync(t => t.UserId == userId);
                if (fcmToken != null)
                {
                    await SendPushAsync(userId, title, content);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send push notification to user {UserId}", userId);
        }
    }

    public async Task SendBatchNotificationAsync(List<Guid> userIds, string title, string content, NotificationType type, Guid? relatedId = null, string? relatedType = null)
    {
        foreach (var userId in userIds)
        {
            await SendNotificationAsync(userId, title, content, type, relatedId, relatedType);
        }
    }

    public async Task SendPushAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null)
    {
        var fcmConfig = _configuration.GetSection("Firebase");
        var projectId = fcmConfig["ProjectId"];
        if (string.IsNullOrEmpty(projectId)) return;

        var fcmToken = await _context.UserFcmTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        if (fcmToken == null) return;

        var client = _httpClientFactory.CreateClient();
        var accessToken = await GetFirebaseAccessTokenAsync(client);
        if (string.IsNullOrEmpty(accessToken)) return;

        var payload = new
        {
            message = new
            {
                token = fcmToken.Token,
                notification = new { title, body },
                data = data ?? new Dictionary<string, string>(),
                android = new { priority = "high" },
                apns = new { payload = new { aps = new { badge = 1, sound = "default" } } }
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await client.PostAsync($"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send", content);
    }

    public async Task SendWechatTemplateAsync(string openid, string templateId, Dictionary<string, string> data)
    {
        var wechatConfig = _configuration.GetSection("WechatMp");
        var appId = wechatConfig["AppId"];
        var appSecret = wechatConfig["AppSecret"];
        if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret)) return;

        var client = _httpClientFactory.CreateClient();
        var accessToken = await GetWechatAccessTokenAsync(client, appId, appSecret);
        if (string.IsNullOrEmpty(accessToken)) return;

        var payload = new
        {
            touser = openid,
            template_id = templateId,
            data
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await client.PostAsync($"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={accessToken}", content);
    }

    private async Task<string?> GetFirebaseAccessTokenAsync(HttpClient client)
    {
        try
        {
            var serviceAccountPath = _configuration.GetSection("Firebase")["ServiceAccountPath"];
            if (string.IsNullOrEmpty(serviceAccountPath) || !File.Exists(serviceAccountPath))
                return null;

            var json = await File.ReadAllTextAsync(serviceAccountPath);
            var serviceAccount = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (serviceAccount == null) return null;

            var privateKey = serviceAccount["private_key"]?.ToString()?.Replace("\\n", "\n");
            var clientEmail = serviceAccount["client_email"]?.ToString();
            if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(clientEmail))
                return null;

            var header = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"));
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    iss = clientEmail,
                    scope = "https://www.googleapis.com/auth/firebase.messaging",
                    aud = "https://oauth2.googleapis.com/token",
                    exp = now + 3600,
                    iat = now
                })));

            using var rsa = System.Security.Cryptography.RSA.Create();
            rsa.ImportFromPem(privateKey);
            var signature = rsa.SignData(
                System.Text.Encoding.UTF8.GetBytes($"{header}.{payload}"),
                System.Security.Cryptography.HashAlgorithmName.SHA256,
                System.Security.Cryptography.RSASignaturePadding.Pkcs1);
            var jwt = $"{header}.{payload}.{Convert.ToBase64String(signature).Replace("+", "-").Replace("/", "_").TrimEnd('=')}";

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = jwt
            });

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            var responseJson = await response.Content.ReadAsStringAsync();
            var tokenResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);
            return tokenResult?["access_token"]?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Firebase access token");
            return null;
        }
    }

    private async Task<string?> GetWechatAccessTokenAsync(HttpClient client, string appId, string appSecret)
    {
        try
        {
            var response = await client.GetStringAsync(
                $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appId}&secret={appSecret}");
            var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            return result?["access_token"]?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get WeChat access token");
            return null;
        }
    }
}
