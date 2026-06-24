namespace CandyHandyman.Application.DTOs;

public class CreatePaymentDto
{
    public Guid OrderId { get; set; }
    public string PaymentMethod { get; set; } = "WechatPay";
}

public class CreateDisputeDto
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EvidenceUrls { get; set; }
}

public class InsurancePurchaseDto
{
    public Guid OrderId { get; set; }
    public string Type { get; set; } = "PersonalInjury";
}