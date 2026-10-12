using MyMc.Extensions;
using MyMc.Models;

namespace MyMc.Processors;

public class FatTableProcessor(FatTableData fatTableData, IndirectFatTableData indirectFatTableData, SuperblockData superblockData) : IProcessor
{
    private readonly FatTableData _fatTableData = fatTableData;
    private readonly IndirectFatTableData _indirectFatTableData = indirectFatTableData;
    private readonly SuperblockData _superblockData = superblockData;

    public void ReadData(FileStream fs)
    {
        var fcInit = _indirectFatTableData.FATClusters[0];
        fs.Seek(fcInit * _superblockData.ClusterLengthInBytes, SeekOrigin.Begin);

        Span<byte> buffer = new byte[_superblockData.ClusterLengthInBytes * 4];
        fs.ReadExactly(buffer);

        Console.WriteLine(string.Join(", ", buffer.ToArray()));

        // while (true)
        // {
        //     var buffer = fs.ReadWord();
        //     var ufc = BitConverter.ToUInt32(buffer);
        //     if (ufc == 0xFFFFFFFF) break;
        //     var fc = BitConverter.ToInt32(buffer);
        //     if (fc >= 0) continue;
        //     _fatTableData.AllocatableClusters.Add(int.MaxValue + fc);
        // }
    }
}
