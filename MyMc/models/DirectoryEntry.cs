namespace MyMc.Models;

public class DirectoryEntry : Entry
{
    public IconSys? IconSys { get; set; }
    public List<DirectoryEntry> Directories { get; set; } = [];
    public List<FileEntry> Files { get; set; } = [];
}
