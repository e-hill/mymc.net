using MyMc.Models;

namespace MyMc.Processors;

public class FatTableProcessor(FatTableData fatTableData) : IProcessor
{
    private readonly FatTableData _fatTableData = fatTableData;

    public void ReadData(FileStream fs)
    {
    }
}
