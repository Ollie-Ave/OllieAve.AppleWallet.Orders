namespace OllieAve.AppleWallet.Orders.Models;

public record WalletAmmount
{
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
}

