using MyMc.Models;
using MyMc.Processors;

namespace MyMc.Services;

public interface IProcessManagerService
{
    IProcessor? GetProcessor(int index);
}

public class ProcessManagerService : IProcessManagerService
{
    private readonly Queue<IProcessor> processors = new();
    private IProcessor activeProcessor;

    public ProcessManagerService(MemoryCard memoryCard)
    {
        processors.Enqueue(new MagicByteProcessor());
        processors.Enqueue(new VersionProcessor(memoryCard));
        activeProcessor = processors.Dequeue();
        activeProcessor.Init();
    }

    public IProcessor? GetProcessor(int index)
    {
        if (index > activeProcessor.EndIndex)
        {
            // check for any failures during processing
            activeProcessor.Verify();

            if (processors.Count == 0)
                return null;

            activeProcessor = processors.Dequeue();
            activeProcessor.Init();
        }

        return activeProcessor;
    }
}
