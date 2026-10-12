using MyMc.Models;

namespace MyMc.Processors;

public class IndirectFatTableProcessor(IndirectFatTableData indirectFatTableData) : IProcessor
{
    private readonly IndirectFatTableData _indirectFatTableData = indirectFatTableData;

    public void ReadData(FileStream fs)
    {
    }
}
