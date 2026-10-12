namespace MyMc.Models;

public class IndirectFatTableData
{
    public List<uint> FATClusters { get; set; } = [];

    public override string ToString()
    {
        return string.Join(", ", FATClusters);
    }
}
