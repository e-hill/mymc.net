using System.Text;
using MyMc.Extensions;
using MyMc.Models;

namespace MyMc.Processors;

public class SuperblockProcessor(SuperblockData superblockData) : IProcessor
{
    private readonly SuperblockData _superblockData = superblockData;

    public void ReadData(FileStream fs)
    {
        ReadMagicString(fs);
        ReadVersion(fs);
        ReadPageLength(fs);
        ReadPagesPerCluster(fs);
        ReadPagesPerBlock(fs);

        // advance past unused bytes
        fs.Seek(ByteLengths.Half, SeekOrigin.Current);

        ReadClustersPerCard(fs);
        ReadAllocOffset(fs);
        ReadAllocEnd(fs);
        ReadRootDirCluster(fs);
        ReadBackupBlock1(fs);
        ReadBackupBlock2(fs);

        // advance past additional unused bytes
        fs.Seek(8, SeekOrigin.Current);

        ReadIfcList(fs);
        ReadBadBlockList(fs);
        ReadCardType(fs);
        ReadCardFlags(fs);
    }

    private void ReadMagicString(FileStream fs)
    {
        Span<byte> buffer = new byte[28];
        fs.ReadExactly(buffer);

        var magicString = Encoding.ASCII.GetString(buffer);
        if (magicString != "Sony PS2 Memory Card Format ")
        {
            throw new Exception("Sony Memory Card Not Formatted");
        }

        _superblockData.Magic = magicString;
    }

    private void ReadVersion(FileStream fs)
    {
        Span<byte> buffer = new byte[12];
        fs.ReadExactly(buffer);

        var version = Encoding.ASCII.GetString(buffer);
        _superblockData.Version = version;
    }

    private void ReadPageLength(FileStream fs)
    {
        var buffer = fs.ReadHalf();
        _superblockData.PageLengthInBytes = BitConverter.ToUInt16(buffer);
    }

    private void ReadPagesPerCluster(FileStream fs)
    {
        var buffer = fs.ReadHalf();
        _superblockData.PagesPerCluster = BitConverter.ToUInt16(buffer);
    }

    private void ReadPagesPerBlock(FileStream fs)
    {
        var buffer = fs.ReadHalf();
        _superblockData.PagesPerBlock = BitConverter.ToUInt16(buffer);
    }

    private void ReadClustersPerCard(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.TotalClusters = BitConverter.ToUInt32(buffer);
    }

    private void ReadAllocOffset(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.AllocOffset = BitConverter.ToUInt32(buffer);
    }

    private void ReadAllocEnd(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.AllocEnd = BitConverter.ToUInt32(buffer);
    }

    private void ReadRootDirCluster(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.RootDirCluster = BitConverter.ToUInt32(buffer);
    }

    private void ReadBackupBlock1(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.BackupBlock1 = BitConverter.ToUInt32(buffer);
    }

    private void ReadBackupBlock2(FileStream fs)
    {
        var buffer = fs.ReadWord();
        _superblockData.BackupBlock2 = BitConverter.ToUInt32(buffer);
    }

    private void ReadIfcList(FileStream fs)
    {
        for (int i = 0; i < 32; i++)
        {
            var buffer = fs.ReadWord();
            var ifc = BitConverter.ToUInt32(buffer);

            if (ifc > 0)
                _superblockData.IndirectFATClusters.Add(ifc);
        }
    }

    private void ReadBadBlockList(FileStream fs)
    {
        for (int i = 0; i < 32; i++)
        {
            var buffer = fs.ReadWord();
            var badBlock = BitConverter.ToUInt32(buffer);

            if (badBlock < 255)
                _superblockData.IndirectFATClusters.Add(badBlock);
        }
    }

    private void ReadCardType(FileStream fs)
    {
        Span<byte> buffer = new byte[1];
        fs.ReadExactly(buffer);
        _superblockData.CardType = buffer[0];
    }

    private void ReadCardFlags(FileStream fs)
    {
        Span<byte> buffer = new byte[1];
        fs.ReadExactly(buffer);
        _superblockData.CardFlags = buffer[0];
    }
}
