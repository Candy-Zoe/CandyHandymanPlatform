namespace CandyHandyman.Application.Interfaces;

public interface IWechatPayService
{
    Task<WechatPayResult> CreatePaymentAsync(string orderNo, decimal amount, string description, string? openid = null);
    Task<bool> HandleNotifyAsync(string body, string signature, string timestamp, string nonce);
    Task<RefundResult> RefundAsync(string orderNo, decimal amount, string reason);
    Task<WechatPayResult> QueryPaymentAsync(string orderNo);
}

public class WechatPayResult
{
    public string PrepayId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string CodeUrl { get; set; } = string.Empty;
}

public class RefundResult
{
    public bool Success { get; set; }
    public string RefundId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
