namespace FileVault.Domain.Aggregates.Files;

/// <summary>
/// Represents the lifecycle state of a file in FileVault.
/// Always use explicit integer values — stored as TINYINT in SQL Server.
/// Never rename or reorder — existing DB rows depend on these values.
/// </summary>
public enum FileStatus
{
    Pending   = 0,   // Uploaded to storage, awaiting virus scan
    Scanning  = 1,   // Kafka event dispatched, scan in progress
    Clean     = 2,   // Passed virus scan — available for download
    Infected  = 3,   // Failed scan — quarantined, download blocked
    Deleted   = 4    // Soft deleted — retained for 30 days, then purged
}
