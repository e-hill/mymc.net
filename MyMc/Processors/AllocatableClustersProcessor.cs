using MyMc.Models;

namespace MyMc.Processors;

public class AllocatableClustersProcessor(AllocatableClustersData allocatableClustersData, SuperblockData superblockData) : IProcessor
{
    private readonly AllocatableClustersData _allocatableClustersData = allocatableClustersData;
    private readonly SuperblockData _superblockData = superblockData;

    public void ReadData(FileStream fs)
    {
        var fcOffset = _superblockData.AllocOffset * _superblockData.ClusterLengthInBytes;
        fs.Seek(fcOffset, SeekOrigin.Begin);

        Span<byte> buffer = new byte[_superblockData.ClusterLengthInBytes * 4];
        fs.ReadExactly(buffer);

        // Console.WriteLine(string.Join(", ", buffer.ToArray()));
    }
}
