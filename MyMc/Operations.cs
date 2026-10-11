using MyMc.Models;
using MyMc.Services;

namespace MyMc;

public class Operations
{
    public async Task<MemoryCard> ReadMemoryCard(byte[]? bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        var memoryCard = new MemoryCard();
        var processManagerService = new ProcessManagerService(memoryCard);

        for (int i = 0; i < bytes.Length; i++)
        {
            var processor = processManagerService.GetProcessor(i);
            if (processor == null) break;
            processor.ProcessByte(bytes[i], i);
        }

        return memoryCard;
    }
}
