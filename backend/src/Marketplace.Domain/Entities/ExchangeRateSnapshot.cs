namespace Marketplace.Domain.Entities;

public sealed class ExchangeRateSnapshot
{
    public Guid Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string TargetCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateOnly SnapshotDate { get; set; }
    public DateTime FetchedAtUtc { get; set; }
}
