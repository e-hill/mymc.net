using MyMc.Models;

namespace MyMc.Processors;

public class IconFileProcessor
{
    private static readonly int[][] dataMap = [
    ];

    public IconFile ReadData(byte[] bytes)
    {
        return new IconFile();
    }

    private static Span<byte> UnpackData(byte[] bytes, int position)
    {
        return bytes.AsSpan(dataMap[position][0], dataMap[position][1]);
    }
}
