using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Security.Cryptography.Pkcs;
using OllieAve.AppleWallet.Orders.Interfaces;
using OllieAve.AppleWallet.Orders.Models;
using System.IO.Compression;
using System.Text;
using OllieAve.AppleWallet.Orders.Extensions;

namespace OllieAve.AppleWallet.Orders.Builders;

public class WalletOrderBuilder : IWalletOrderBuilder
{
    private static readonly JsonSerializerOptions SerialiserOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private Dictionary<string, byte[]>? images = null;

    private WalletOrder? order = null;

    private X509Certificate2? certificate = null;

    public static WalletOrderBuilder Initialize()
    {
        return new();
    }

    public byte[] BuildSignedPackage()
    {
        ValidateCertificate();
        ValidateOrder();
        ValidateImages(order);

        var orderBytes = JsonSerializer.SerializeToUtf8Bytes(order, SerialiserOptions);
        var manifestJson = GetManifestJson(orderBytes, images);
        var signature = GetSignature(manifestJson, certificate);
        var zippedOrder = GetZippedOrder(orderBytes, manifestJson, signature, images);

        return zippedOrder;
    }

    private static byte[] GetZippedOrder(
            byte[] orderBytes,
            byte[] manifestJson,
            byte[] signature,
            Dictionary<string, byte[]> images)
    {
        using MemoryStream ms = new();
        using (ZipArchive zip = new(ms, ZipArchiveMode.Create, true))
        {
            zip.AddEntry("order.json", orderBytes)
                .AddEntry("manifest.json", manifestJson, new UTF8Encoding(false))
                .AddEntry("signature", signature);

            foreach (KeyValuePair<string, byte[]> image in images)
            {
                zip.AddEntry(image.Key, image.Value);
            }
        }

        return ms.ToArray();
    }


    private static byte[] GetSignature(byte[] manifestJson, X509Certificate2 certificate)
    {
        const string SHA1 = "1.3.14.3.2.26";

        CmsSigner signer = new(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
        {
            IncludeOption = X509IncludeOption.EndCertOnly,
            DigestAlgorithm = new Oid(SHA1),
        };

        signer.SignedAttributes.Add(new Pkcs9SigningTime());

        byte[] content = manifestJson;
        SignedCms cms = new(new ContentInfo(content), detached: true);

        cms.ComputeSignature(signer);
        byte[] result = cms.Encode();

        return result;
    }

    public IWalletOrderBuilder SetOrder(WalletOrder order)
    {
        this.order = order;

        return this;
    }

    public IWalletOrderBuilder SetSigningCertificate(X509Certificate2 certificate)
    {
        this.certificate = null;

        return this;
    }

    public IWalletOrderBuilder SetSigningCertificate(byte[] certificateBytes, string certificatePassword)
    {
        certificate = X509CertificateLoader.LoadPkcs12(certificateBytes, certificatePassword);

        return this;
    }


    public IWalletOrderBuilder AddImages(Dictionary<string, byte[]> images)
    {
        this.images = images;

        return this;
    }

    [MemberNotNull(nameof(order))]
    private void ValidateOrder()
    {
        if (order is null)
        {
            throw new ArgumentNullException("The order is unset. This is required.");
        }

        if (string.IsNullOrWhiteSpace(order.WebServiceURL) == false
            && string.IsNullOrWhiteSpace(order.AuthenticationToken) == true)
        {
            throw new ArgumentException($"If the {nameof(order.WebServiceURL)} is set, the {nameof(order.AuthenticationToken)} must also be set.");
        }
    }

    [MemberNotNull(nameof(images))]
    private void ValidateImages(WalletOrder order)
    {
        List<string> imagesInOrder = [
            ..order.LineItems.Select(x => x.Image).Distinct(),
            order.Merchant.Logo,
        ];

        if (imagesInOrder.Count == 0)
        {
            images = [];
            return;
        }

        if (images is null)
        {
            throw new ArgumentException("There are images set in the order project but no source for those images set.");
        }


        foreach (var imageInOrder in imagesInOrder)
        {
            bool imageSourceFound = images.TryGetValue(imageInOrder, out _);

            if (imageSourceFound == false)
            {
                throw new ArgumentException($"'{imageInOrder}' was listed in the order source but the source for the image was not found.");
            }
        }
    }

    [MemberNotNull(nameof(certificate))]
    private void ValidateCertificate()
    {
        if (certificate is null)
        {
            throw new ArgumentNullException("The certificate has not been set. This is required to sign the package.");
        }

        if (!certificate.HasPrivateKey)
        {
            throw new InvalidOperationException("The supplied certificate does not contain a private key. See documentation for more details.");
        }
    }

    private static byte[] GetManifestJson(byte[] orderJson, Dictionary<string, byte[]> images)
    {
        Dictionary<string, string> manifest = new()
        {
            ["order.json"] = Convert.ToHexString(SHA256.HashData(orderJson)).ToLowerInvariant(),
        };

        foreach (KeyValuePair<string, byte[]> image in images)
        {
            manifest.Add(image.Key, Convert.ToHexString(SHA256.HashData(image.Value)).ToLowerInvariant());
        }

        return JsonSerializer.SerializeToUtf8Bytes(manifest, SerialiserOptions);
    }
}
