using System.Text;

namespace MyMc.Models;

public class AllocatableClustersData
{
    public List<DirectoryEntry> Directories { get; set; } = [];

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (var entry in Directories)
        {
            sb.AppendLine(entry.Name);
            foreach (var file in entry.Files)
            {
                sb.AppendLine($"  - {file.Name}");
                if (file.Name == IconSys.FileName && entry.IconSys != null)
                {
                    sb.AppendLine($"    - {entry.IconSys.Title.Part1}");
                    if (!string.IsNullOrEmpty(entry.IconSys.Title.Part2))
                        sb.AppendLine($"    - {entry.IconSys.Title.Part2}");
                }
            }
        }

        return sb.ToString();
    }
}
