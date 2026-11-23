using MyMc.Extensions;
using MyMc.Models;
using MyMc.Models.Flags;
using MyMc.SubProcessors;

namespace MyMc.Processors;

public class AllocatableClustersProcessor(AllocatableClustersData allocatableClustersData, FatTableData fatTableData, SuperblockData superblockData) : IProcessor
{
    private readonly AllocatableClustersData _allocatableClustersData = allocatableClustersData;
    private readonly FatTableData _fatTableData = fatTableData;
    private readonly SuperblockData _superblockData = superblockData;

    public void ReadData(FileStream fs)
    {
        int currentCluster = 0;
        while (currentCluster >= 0)
        {
            for (int i = 0; i < _superblockData.PagesPerCluster; i++)
            {
                ReadEntry(fs, currentCluster, i);
            }

            var fat = _fatTableData.Entries[currentCluster];
            currentCluster = fat.Flag == FatTableFlag.NEXT_CLUSTER ? fat.NextCluster : -1;
        }

        ReadDirectories(fs, _allocatableClustersData.Directories);
    }

    private void ReadDirectories(FileStream fs, List<DirectoryEntry> directories)
    {
        foreach (var dir in directories)
        {
            if (dir.Name == "." || dir.Name == "..") continue;
            var currentCluster = (int)dir.Cluster;
            while (currentCluster >= 0)
            {
                for (int i = 0; i < _superblockData.PagesPerCluster; i++)
                {
                    ReadEntry(fs, currentCluster, i, dir);
                }

                var fat = _fatTableData.Entries[currentCluster];
                currentCluster = fat.Flag == FatTableFlag.NEXT_CLUSTER ? fat.NextCluster : -1;
            }

            ReadDirectories(fs, dir.Directories);
        }
    }

    private void ReadEntry(FileStream fs, int cluster, int page)
    {
        var clusterOffset = (_superblockData.AllocOffset + cluster) * _superblockData.ClusterLengthInBytes;
        var pageOffset = _superblockData.PageLengthInBytes * page;
        var position = clusterOffset + pageOffset;

        fs.Seek(position, SeekOrigin.Begin);

        var mode = ReadMode(fs);

        if (mode == DirectoryEntryMode.INVALID) return;
        else if (mode.HasFlag(DirectoryEntryMode.DF_DIRECTORY))
        {
            var directoryEntry = EntrySubProcessor.ReadDirectory(fs);
            _allocatableClustersData.Directories.Add(directoryEntry);
        }
    }

    private void ReadEntry(FileStream fs, int cluster, int page, DirectoryEntry directory)
    {
        var clusterOffset = (_superblockData.AllocOffset + cluster) * _superblockData.ClusterLengthInBytes;
        var pageOffset = _superblockData.PageLengthInBytes * page;
        var position = clusterOffset + pageOffset;

        fs.Seek(position, SeekOrigin.Begin);

        var mode = ReadMode(fs);

        if (mode == DirectoryEntryMode.INVALID) return;
        else if (mode.HasFlag(DirectoryEntryMode.DF_DIRECTORY))
        {
            var directoryEntry = EntrySubProcessor.ReadDirectory(fs);
            directory.Directories.Add(directoryEntry);
        }
        else
        {
            var fileEntry = EntrySubProcessor.ReadFile(fs);
            var fileCluster = (int)fileEntry.Cluster;

            while (fileCluster >= 0)
            {
                var fileClusterOffset = (_superblockData.AllocOffset + fileCluster) * _superblockData.ClusterLengthInBytes;
                fs.Seek(fileClusterOffset, SeekOrigin.Begin);

                for (int i = 0; i < _superblockData.PagesPerCluster; i++)
                {
                    Span<byte> buffer = new byte[_superblockData.PageDataLengthInBytes];
                    fs.ReadExactly(buffer);
                    fileEntry.Data.AddRange(buffer);
                    fs.Seek(_superblockData.PageSpareAreaLengthInBytes, SeekOrigin.Current);
                }

                var fat = _fatTableData.Entries[fileCluster];
                fileCluster = fat.Flag == FatTableFlag.NEXT_CLUSTER ? fat.NextCluster : -1;
            }

            int eofIndex = fileEntry.Data.FindLastIndex(b => b != 0xFF) + 1;
            fileEntry.Data.RemoveRange(eofIndex, fileEntry.Data.Count - eofIndex);

            if (fileEntry.Name == IconSys.FileName)
            {
                directory.IconSys = new IconSysProcessor().ReadData([.. fileEntry.Data]);
            }
            else if (fileEntry.Name.EndsWith(".ico"))
            {
                directory.IconFile = new IconFileProcessor().ReadData([.. fileEntry.Data]);
            }

            directory.Files.Add(fileEntry);
        }
    }

    private static DirectoryEntryMode ReadMode(FileStream fs)
    {
        var modeBuffer = fs.ReadWord();
        return (DirectoryEntryMode)BitConverter.ToUInt32(modeBuffer);
    }
}
