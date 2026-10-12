using MyMc.Models;

namespace MyMc.Processors;

public class BackupBlock2Processor(BackupBlockData backupBlockData) : IProcessor
{
    private readonly BackupBlockData _backupBlockData = backupBlockData;

    public void ReadData(FileStream fs)
    {
    }
}
