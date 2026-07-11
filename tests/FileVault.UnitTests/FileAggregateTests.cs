using FileVault.Domain.Aggregates.Files;

namespace FileVault.UnitTests;

public class FileAggregateTests
{
    [Fact]
    public void MarkAsScanning_And_Clean_Transitions_StatusAndUpdatesTimestamp()
    {
        var aggregate = new FileAggregate(
            Guid.NewGuid(),
            "photo.png",
            new StoragePath("uploads/photo.png"),
            new ContentType("image/png"),
            new FileSize(1024));

        aggregate.MarkAsScanning();
        Assert.Equal(FileStatus.Scanning, aggregate.Status);

        aggregate.MarkAsClean();
        Assert.Equal(FileStatus.Clean, aggregate.Status);
        Assert.True(aggregate.UpdatedAt >= aggregate.CreatedAt);
    }
}
