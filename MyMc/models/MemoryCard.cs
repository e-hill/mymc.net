using System.Text;

namespace MyMc.Models;

public partial class MemoryCard
{
    public SuperblockData Superblock { get; set; } = new SuperblockData();
    public IndirectFatTableData IndirectFatTable { get; set; } = new IndirectFatTableData();
    public FatTableData FatTable { get; set; } = new FatTableData();
    public AllocatableClustersData AllocatableClusters { get; set; } = new AllocatableClustersData();
    public ReservedClustersData ReservedClusters { get; set; } = new ReservedClustersData();
    public BackupBlockData BackupBlock2 { get; set; } = new BackupBlockData();
    public BackupBlockData BackupBlock1 { get; set; } = new BackupBlockData();

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Superblock: " + Superblock.ToString());
        stringBuilder.AppendLine("Indirect Fat Table: " + IndirectFatTable.ToString());
        stringBuilder.AppendLine("Fat Table: " + FatTable.ToString());
        stringBuilder.AppendLine("Allocatable Clusters: " + AllocatableClusters.ToString());
        stringBuilder.AppendLine("Reserved Clusters: " + ReservedClusters.ToString());
        stringBuilder.AppendLine("Backup Block 2: " + BackupBlock2.ToString());
        stringBuilder.AppendLine("Backup Block 1: " + BackupBlock1.ToString());
        return stringBuilder.ToString();
    }
}

