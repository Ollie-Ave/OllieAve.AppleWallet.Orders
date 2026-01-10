namespace OllieAve.AppleWallet.Orders.Extensions;

using System.IO;
using System.IO.Compression;
using System.Text;

internal static class ZipArchiveExtensions
{
    public static ZipArchive AddEntry(this ZipArchive zip, string name, string content)
    {
        ZipArchiveEntry entry = zip.CreateEntry(name);

        using Stream stream = entry.Open();

        using StreamWriter writer = new(stream, Encoding.UTF8);
        writer.Write(content);

        return zip;
    }

    public static ZipArchive AddEntry(this ZipArchive zip, string name, byte[] content, UTF8Encoding encoding)
    {
        string contentString = Encoding.UTF8.GetString(content);

        ZipArchiveEntry entry = zip.CreateEntry(name);
        using Stream stream = entry.Open();
        using StreamWriter writer = new(stream, encoding);
        writer.Write(contentString);

        return zip;
    }

    public static ZipArchive AddEntry(this ZipArchive zip, string name, byte[] content)
    {
        ZipArchiveEntry entry = zip.CreateEntry(name);
        using Stream stream = entry.Open();
        stream.Write(content);

        return zip;
    }
}

