namespace OllieAve.AppleWallet.Orders.Models;

public record WalletPayment
{
    public required AppleWalletAmmount Total { get; init; }
    public required string Status { get; init; }
    public required List<AppleWalletSummaryItem> SummaryItems { get; init; }
}
