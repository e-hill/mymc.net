using System.Text;
using MyMc.Extensions;
using MyMc.Models;

namespace MyMc.SubProcessors;

public class EntrySubProcessor()
{
    public static DirectoryEntry ReadDirectory(FileStream fs)
    {
        var entry = new DirectoryEntry();
        return ReadData(fs, entry);
    }

    public static FileEntry ReadFile(FileStream fs)
    {
        var entry = new FileEntry();
        return ReadData(fs, entry);
    }

    private static T ReadData<T>(FileStream fs, T entry) where T : Entry
    {
        var buffer = fs.ReadWord();
        entry.Length = BitConverter.ToUInt32(buffer);

        buffer = new byte[8];
        fs.ReadExactly(buffer);

        var creationTime = new DateTimeOffset(
            BitConverter.ToUInt16([buffer[6], buffer[7]]), // year
            buffer[5], // month
            buffer[4], // day
            buffer[3], // hour
            buffer[2], // minute
            buffer[1], TimeSpan.FromHours(9)); // second

        entry.CreationTime = creationTime.ToUniversalTime();

        buffer = fs.ReadWord();
        entry.Cluster = BitConverter.ToUInt32(buffer);

        buffer = fs.ReadWord();
        entry.DirEntry = BitConverter.ToUInt32(buffer);

        buffer = new byte[8];
        fs.ReadExactly(buffer);

        var modifiedTime = new DateTimeOffset(
            BitConverter.ToUInt16([buffer[6], buffer[7]]), // year
            buffer[5], // month
            buffer[4], // day
            buffer[3], // hour
            buffer[2], // minute
            buffer[1], TimeSpan.FromHours(9)); // second

        entry.ModifiedTime = modifiedTime.ToUniversalTime();

        buffer = fs.ReadWord();
        entry.Attribute = BitConverter.ToUInt32(buffer);

        fs.Seek(28, SeekOrigin.Current);

        buffer = new byte[32];
        fs.ReadExactly(buffer);
        entry.Name = Encoding.ASCII.GetString(buffer);

        return entry;
    }
}
