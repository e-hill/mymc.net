using System.Text;
using MyMc.Models;

namespace MyMc.Processors;

public class VersionProcessor(MemoryCard memoryCard)
{
    public int StartIndex => 28;
    public int EndIndex => 39;

    private readonly MemoryCard _memoryCard = memoryCard;

    private readonly byte[] buffer = new byte[12];

    public void Init()
    {
        Console.WriteLine("Begin verifying the version information...");
    }

    public void ProcessByte(byte b, int index)
    {
        buffer[index - StartIndex] = b;
    }

    public void Verify()
    {
        _memoryCard.Superblock.Version = Encoding.ASCII.GetString(buffer);
        Console.WriteLine("Successfully verified the version information.");
    }
}
