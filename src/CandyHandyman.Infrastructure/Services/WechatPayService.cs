using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using CandyHandyman.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CandyHandyman.Infrastructure.Services;

public class WechatPayService : IWechatPayService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public WechatPayService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<WechatPayResult> CreatePaymentAsync(string orderNo, decimal amount, string description, string? openid = null)
    {
        var config = _configuration.GetSection("WechatPay");
        var mchId = config["MchId"]!;
        var appId = config["AppId"]!;
        var serialNo = config["CertSerialNo"]!;
        var apiV3Key = config["ApiV3Key"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"WECHATPAY2-SHA256-RSA2048 {await GetAccessTokenAsync(client, config)}");

        var notifyUrl = config["NotifyUrl"];
        var amountInCent = (int)(amount * 100);

        var payload = new Dictionary<string, object>
        {
            ["appid"] = appId,
            ["mchid"] = mchId,
            ["description"] = description,
            ["out_trade_no"] = orderNo,
            ["notify_url"] = notifyUrl,
            ["amount"] = new { total = amountInCent, currency = "CNY" }
        };

        if (!string.IsNullOrEmpty(openid))
        {
            payload["payer"] = new { openid };
        }

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi", content);
        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);

        return new WechatPayResult
        {
            PrepayId = result?.GetValueOrDefault("prepay_id")?.ToString() ?? "",
            PaymentUrl = result?.GetValueOrDefault("h5_url")?.ToString() ?? "",
            CodeUrl = result?.GetValueOrDefault("code_url")?.ToString() ?? ""
        };
    }

    public async Task<bool> HandleNotifyAsync(string body, string signature, string timestamp, string nonce)
    {
        try
        {
            var config = _configuration.GetSection("WechatPay");
            var certPath = config["CertPath"];

            if (string.IsNullOrEmpty(certPath) || !File.Exists(certPath))
                return false;

            var cert = new X509Certificate2(certPath);
            var certPublicKey = cert.GetRSAPublicKey();

            var message = $"{timestamp}\n{nonce}\n{body}\n";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var signatureBytes = Convert.FromBase64String(signature);

            var isValid = certPublicKey!.VerifyData(
                messageBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            return isValid;
        }
        catch
        {
            return false;
        }
    }

    public async Task<RefundResult> RefundAsync(string orderNo, decimal amount, string reason)
    {
        var config = _configuration.GetSection("WechatPay");
        var mchId = config["MchId"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"WECHATPAY2-SHA256-RSA2048 {await GetAccessTokenAsync(client, config)}");

        var refundNo = $"REF{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        var amountInCent = (int)(amount * 100);

        var payload = new
        {
            out_trade_no = orderNo,
            out_refund_no = refundNo,
            reason,
            amount = new
            {
                refund = amountInCent,
                total = amountInCent,
                currency = "CNY"
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.mch.weixin.qq.com/v3/refund/domestic/refunds", content);
        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);

        return new RefundResult
        {
            Success = response.IsSuccessStatusCode,
            RefundId = result?.GetValueOrDefault("refund_id")?.ToString() ?? "",
            Message = response.IsSuccessStatusCode ? "退款成功" : "退款失败"
        };
    }

    public async Task<WechatPayResult> QueryPaymentAsync(string orderNo)
    {
        var config = _configuration.GetSection("WechatPay");
        var mchId = config["MchId"]!;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"WECHATPAY2-SHA256-RSA2048 {await GetAccessTokenAsync(client, config)}");

        var response = await client.GetAsync($"https://api.mch.weixin.qq.com/v3/pay/transactions/out-trade-no/{orderNo}?mchid={mchId}");
        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);

        return new WechatPayResult
        {
            PrepayId = result?.GetValueOrDefault("prepay_id")?.ToString() ?? "",
            PaymentUrl = result?.GetValueOrDefault("h5_url")?.ToString() ?? "",
            CodeUrl = result?.GetValueOrDefault("code_url")?.ToString() ?? ""
        };
    }

    private async Task<string> GetAccessTokenAsync(HttpClient client, IConfigurationSection config)
    {
        var mchId = config["MchId"]!;
        var serialNo = config["CertSerialNo"]!;
        var privateKeyPath = config["KeyPath"]!;

        if (!File.Exists(privateKeyPath))
            return "";

        var privateKey = File.ReadAllText(privateKeyPath);
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"));
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            iss = mchId,
            iat = now,
            exp = now + 300,
            nonce = Guid.NewGuid().ToString("N")
        })));

        var data = Encoding.UTF8.GetBytes($"{header}.{payload}");
        var signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var jwt = $"{header}.{payload}.{Convert.ToBase64String(signature).Replace("+", "-").Replace("/", "_").TrimEnd('=')}";

        return jwt;
    }
}
