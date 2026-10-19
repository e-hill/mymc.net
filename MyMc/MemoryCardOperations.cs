using MyMc.Models;
using MyMc.Services;

namespace MyMc;

public class MemoryCardOperations
{
    public static MemoryCard Read(FileStream fs)
    {
        VerifyHostArchitecture();

        var memoryCard = new MemoryCard();
        var processManagerService = new ProcessManagerService(memoryCard);

        while (true)
        {
            var processor = processManagerService.GetProcessor();
            if (processor == null) break;
            processor.ReadData(fs);
        }

        return memoryCard;
    }

    private static void VerifyHostArchitecture()
    {
        if (!BitConverter.IsLittleEndian)
            throw new NotSupportedException("This program requires a little-endian CPU.");
    }
}
