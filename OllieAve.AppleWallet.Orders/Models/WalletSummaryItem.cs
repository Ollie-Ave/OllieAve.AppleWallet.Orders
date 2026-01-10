namespace OllieAve.AppleWallet.Orders.Models;

public record WalletSummaryItem
{
    public required string Label { get; init; }
    public required WalletAmmount Value { get; init; }
}
