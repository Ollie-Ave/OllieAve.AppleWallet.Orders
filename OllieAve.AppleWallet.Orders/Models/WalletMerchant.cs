namespace OllieAve.AppleWallet.Orders.Models;

public record WalletMerchant
{
    public required string DisplayName { get; init; }
    public required string MerchantIdentifier { get; init; }
    public required string Url { get; init; }
    public required string Logo { get; init; }
}
