namespace MyMc.Models;

public class DirectoryEntry : Entry
{
    public List<DirectoryEntry> Directories { get; set; } = [];
    public List<FileEntry> Files { get; set; } = [];
}
