using FileVault.Domain.Common;

namespace FileVault.Domain.Aggregates.Files;

public sealed class FileSize : ValueObject
{
    public long Bytes { get; }

    public FileSize(long bytes)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes), "File size cannot be negative.");

        Bytes = bytes;
    }

    public override string ToString() => $"{Bytes} bytes";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Bytes;
    }
}
