namespace OllieAve.AppleWallet.Orders.Models;

public record WalletPayment
{
    public required WalletAmmount Total { get; init; }
    public required string Status { get; init; }
    public List<WalletSummaryItem>? SummaryItems { get; init; }
}
