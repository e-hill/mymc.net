using System.Text;

namespace MyMc.Models;

public class SuperblockData
{
    public string Magic { get; set; } = ""; // Identifies the card as being formatted. Set to the ASCII string "Sony PS2 Memory Card Format ".
    public string Version { get; set; } = ""; // Version number of the format used.
    public ushort PageLengthInBytes { get; set; } = 512; // Size in bytes of a memory card page.
    public ushort PagesPerCluster { get; set; } = 2; // The number of pages in a cluster.
    public ushort PagesPerBlock { get; set; } = 16; // The number of pages in an erase block.
    public uint TotalClusters { get; set; } = 8192; // The total size of the card in clusters.
    public uint AllocOffset { get; set; } = 41; // Cluster offset of the first allocatable cluster.
    public uint AllocEnd { get; set; } = 8135; // The cluster after the highest allocatable cluster. Relative to alloc_offset.
    public uint RootDirCluster { get; set; } = 0; // First cluster of the root directory. Relative to alloc_offset.
    public uint BackupBlock1 { get; set; } = 1023; // Erase block used as a backup area during programming.
    public uint BackupBlock2 { get; set; } = 1022; // This block should be erased to all ones.
    public List<uint> IndirectFATClusterList { get; set; } = []; // List of indirect FAT clusters.
    public List<uint> BadBlockList { get; set; } = []; // List of erase blocks that have errors and shouldn't be used.
    public byte CardType { get; set; } = 2; // Memory card type.
    public byte CardFlags { get; set; } = 0x52;

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Magic String: " + Magic);
        stringBuilder.AppendLine("Version: " + Version);
        stringBuilder.AppendLine("Page Length: " + PageLengthInBytes);
        stringBuilder.AppendLine("Pages Per Cluster: " + PagesPerCluster);
        stringBuilder.AppendLine("Pages Per Block: " + PagesPerBlock);
        stringBuilder.AppendLine("Total Clusters: " + TotalClusters);
        stringBuilder.AppendLine("Alloc Offset: " + AllocOffset);
        stringBuilder.AppendLine("Alloc End: " + AllocEnd);
        stringBuilder.AppendLine("Root Dir Cluster: " + RootDirCluster);
        stringBuilder.AppendLine("Backup Block 1: " + BackupBlock1);
        stringBuilder.AppendLine("Backup Block 2: " + BackupBlock2);
        stringBuilder.AppendLine("Indirect FAT Cluster List: " + string.Join(", ", IndirectFATClusterList));
        stringBuilder.AppendLine("Bad Block List: " + string.Join(", ", BadBlockList));
        stringBuilder.AppendLine("Card Type: " + CardType);
        stringBuilder.AppendLine("Card Flags: " + CardFlags);
        return stringBuilder.ToString();
    }
}
