using System.Security.Cryptography.X509Certificates;
using OllieAve.AppleWallet.Orders.Models;

namespace OllieAve.AppleWallet.Orders.Interfaces;

public interface IWalletOrderBuilder
{
    IWalletOrderBuilder SetOrder(WalletOrder order);

    IWalletOrderBuilder SetSigningCertificate(X509Certificate2 certificate);

    IWalletOrderBuilder SetSigningCertificate(byte[] certificateBytes, string certificatePassword);

    IWalletOrderBuilder AddImages(Dictionary<string, byte[]> images);

    byte[] BuildSignedPackage();
}
