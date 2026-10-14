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
                    var fte = next == -1 ? new FatTableEntry { Flag = FatTableFlag.LAST_CLUSTER }
                        : next < -1 ? new FatTableEntry { Flag = FatTableFlag.NEXT_CLUSTER, NextCluster = int.MaxValue + next + 1 }
                        : new FatTableEntry { Flag = FatTableFlag.FREE };

                    _fatTableData.Entries.Add(index, fte);
                    index++;
                }
            }
        }
    }
}
