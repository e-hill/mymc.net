using MyMc.Models;

namespace MyMc.Processors;

public class AllocatableClustersProcessor(AllocatableClustersData allocatableClustersData) : IProcessor
{
    private readonly AllocatableClustersData _allocatableClustersData = allocatableClustersData;

    public void ReadData(FileStream fs)
    {
    }
}
