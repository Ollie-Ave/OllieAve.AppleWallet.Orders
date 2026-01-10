namespace OllieAve.AppleWallet.Orders.Models;

public record WalletItem
{
    public required WalletAmmount Price { get; init; }
    public required int Quantity { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string Image { get; init; }
}
