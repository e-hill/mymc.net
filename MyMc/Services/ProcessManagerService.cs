using MyMc.Models;
using MyMc.Processors;

namespace MyMc.Services;

public interface IProcessManagerService
{
    IProcessor? GetProcessor();
}

public class ProcessManagerService : IProcessManagerService
{
    private readonly Queue<IProcessor> processors = new();

    public ProcessManagerService(MemoryCard memoryCard)
    {
        processors.Enqueue(new SuperblockProcessor(memoryCard.Superblock));
        processors.Enqueue(new IndirectFatTableProcessor(memoryCard.IndirectFatTable, memoryCard.Superblock));
        processors.Enqueue(new FatTableProcessor(memoryCard.FatTable, memoryCard.IndirectFatTable, memoryCard.Superblock));
        processors.Enqueue(new AllocatableClustersProcessor(memoryCard.AllocatableClusters, memoryCard.Superblock));
        processors.Enqueue(new ReservedClustersProcessor(memoryCard.ReservedClusters));
        processors.Enqueue(new BackupBlock2Processor(memoryCard.BackupBlock2));
        processors.Enqueue(new BackupBlock1Processor(memoryCard.BackupBlock1));
    }

    public IProcessor? GetProcessor()
    {
        processors.TryDequeue(out IProcessor? result);
        return result;
    }
}
