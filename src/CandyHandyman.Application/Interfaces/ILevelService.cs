namespace CandyHandyman.Application.Interfaces;

public interface ILevelService
{
    Task EvaluateAndUpgradeAsync(Guid handymanId);
}
