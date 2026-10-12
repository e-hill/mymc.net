using MyMc.Extensions;
using MyMc.Models;

namespace MyMc.Processors;

public class IndirectFatTableProcessor(IndirectFatTableData indirectFatTableData, SuperblockData superblockData) : IProcessor
{
    private readonly IndirectFatTableData _indirectFatTableData = indirectFatTableData;
    private readonly SuperblockData _superblockData = superblockData;

    public void ReadData(FileStream fs)
    {
        foreach (var ifc in _superblockData.IndirectFATClusters)
        {
            fs.Seek(ifc * _superblockData.ClusterLengthInBytes, SeekOrigin.Begin);

            while (true)
            {
                var buffer = fs.ReadWord();
                var fc = BitConverter.ToUInt32(buffer);
                if (fc == 0xFFFFFFFF) break;
                _indirectFatTableData.FATClusters.Add(fc);
            }
        }
    }
}
