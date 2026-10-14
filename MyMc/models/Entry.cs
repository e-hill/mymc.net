using System.Text;

namespace MyMc.Models;

public class Entry
{
    public uint Length { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public uint Cluster { get; set; }
    public uint DirEntry { get; set; }
    public DateTimeOffset ModifiedTime { get; set; }
    public uint Attribute { get; set; }
    public string Name { get; set; } = "";

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Length/Entries: " + Length);
        stringBuilder.AppendLine("Creation Time: " + CreationTime);
        stringBuilder.AppendLine("Cluster: " + Cluster);
        stringBuilder.AppendLine("Dir Entry: " + DirEntry);
        stringBuilder.AppendLine("Modified Time: " + ModifiedTime);
        stringBuilder.AppendLine("Attribute: " + Attribute);
        stringBuilder.AppendLine("Name: " + Name);
        return stringBuilder.ToString();
    }
}
