using System.IO;
using MyMc.Models;
using MyMc.Services;

namespace MyMc;

public class Operations
{
    public static MemoryCard ReadMemoryCard(FileStream fs)
    {
        var memoryCard = new MemoryCard();
        var processManagerService = new ProcessManagerService(memoryCard);

        while (true)
        {
            var processor = processManagerService.GetProcessor();
            if (processor == null) break;
            processor.ReadData(fs);
        }

        Console.WriteLine(memoryCard.ToString());
        return memoryCard;
    }
}
