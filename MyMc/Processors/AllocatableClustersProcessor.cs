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

        foreach (var dir in _allocatableClustersData.Directories)
        {
            if (dir.Name == "." || dir.Name == "..") continue;

            currentCluster = (int)dir.Cluster;
            while (currentCluster >= 0)
            {
                for (int i = 0; i < _superblockData.PagesPerCluster; i++)
                {
                    ReadEntry(fs, currentCluster, i, dir);
                }

                var fat = _fatTableData.Entries[currentCluster];
                currentCluster = fat.Flag == FatTableFlag.NEXT_CLUSTER ? fat.NextCluster : -1;
            }
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
            Console.WriteLine(directoryEntry);
        }
    }

    private void ReadEntry(FileStream fs, int cluster, int page, DirectoryEntry directory)
    {
        var clusterOffset = (_superblockData.AllocOffset + cluster) * _superblockData.ClusterLengthInBytes;
        var pageOffset = _superblockData.PageLengthInBytes * page;
        var position = clusterOffset + pageOffset;

        fs.Seek(position, SeekOrigin.Begin);

        var mode = ReadMode(fs);
        Console.WriteLine(mode);

        if (mode == DirectoryEntryMode.INVALID) return;
        else if (mode.HasFlag(DirectoryEntryMode.DF_DIRECTORY))
        {
            var directoryEntry = EntrySubProcessor.ReadDirectory(fs);
            directory.Directories.Add(directoryEntry);
            Console.WriteLine(directoryEntry);
        }
        else
        {
            var fileEntry = EntrySubProcessor.ReadFile(fs);
            directory.Files.Add(fileEntry);
            Console.WriteLine(fileEntry);
        }
    }

    private DirectoryEntryMode ReadMode(FileStream fs)
    {
        var modeBuffer = fs.ReadWord();
        return (DirectoryEntryMode)BitConverter.ToUInt32(modeBuffer);
    }
}
