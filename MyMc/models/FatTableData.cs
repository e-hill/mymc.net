namespace MyMc.Models;

public class FatTableData
{
    public Dictionary<int, FatTableEntry> Entries { get; set; } = [];

    public override string ToString()
    {
        return $"{Entries.Count} Entries";
    }
}

public class FatTableEntry
{
    public int NextCluster { get; set; } = -1;

    public FatTableFlag Flag { get; set; } = FatTableFlag.FREE;

    public override string ToString()
    {
        return $"{NextCluster} {Flag}";
    }
}

public enum FatTableFlag
{
    FREE,
    NEXT_CLUSTER,
    LAST_CLUSTER
}

