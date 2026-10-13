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
        var index = 0;

        foreach (var fc in _indirectFatTableData.FATClusters)
        {
            fs.Seek(fc * _superblockData.ClusterLengthInBytes, SeekOrigin.Begin);
            for (int i = 0; i < _superblockData.PagesPerCluster; i++)
            {
                Span<byte> buffer = new byte[_superblockData.PageLengthInBytes];
                fs.ReadExactly(buffer);

                // loop through each entry on the page, skipping the spare area
                for (int j = 0; j < buffer.Length - 16; j += ByteLengths.Word)
                {
                    var entry = buffer.Slice(j, ByteLengths.Word);
                    var next = BitConverter.ToInt32(entry);
                    if (next < -1) next = int.MinValue - next * -1;
                    else if (next > 0) next = 0;
                    // _fatTableData.AllocatableClusters.Add(cluster);

                    /*
                        0 = FREE
                        -1 = xFFFF (EOf)
                        >0 = cluster number
                    */

                    Console.WriteLine($"{index}. {entry[0]}, {entry[1]}, {entry[2]}, {entry[3]}; {next}");
                    index++;
                }
            }
        }
    }
}
