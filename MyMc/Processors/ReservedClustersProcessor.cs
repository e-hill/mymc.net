using MyMc.Models;

namespace MyMc.Processors;

public class ReservedClustersProcessor(ReservedClustersData reservedClustersData) : IProcessor
{
    private readonly ReservedClustersData _reservedClustersData = reservedClustersData;

    public void ReadData(FileStream fs)
    {
    }
}
