namespace OllieAve.AppleWallet.Orders.Models;

public record WalletOrder
{
    public required int SchemaVersion { get; init; }
    public required string OrderTypeIdentifier { get; init; }
    public required string MerchantIdentifier { get; init; }
    public required string OrderIdentifier { get; init; }
    public required string OrderType { get; init; }
    public required string OrderNumber { get; init; }
    public required string CreatedAt { get; init; }
    public required string UpdatedAt { get; init; }
    public required string Status { get; init; }
    public required WalletMerchant Merchant { get; init; }
    public required WalletPayment Payment { get; init; }
    public required List<WalletItem> LineItems { get; init; }
    public required string OrderManagementURL { get; init; }
    public required string WebServiceURL { get; init; }
    public required string AuthenticationToken { get; init; }

}
