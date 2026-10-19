using System.Text;
using MyMc.Models;
using MyMc.Utilities;

namespace MyMc.Processors;

public class IconSysProcessor()
{
    private static readonly int[][] dataMap = [
        [0, 4], // magic
        [6, 2], // title line offset
        [12, 4], // bg transparency
        [16, 16], [32, 16], [48, 16], [64, 16], // bg colors
        [80, 16], [96, 16], [112, 16],  // light directions
        [128, 16], [144, 16], [160, 16], // light colors
        [176, 16], // ambient light color
        [192, 68], // title sjis
        [260, 64], // file icon normal
        [324, 64], // file icon copy
        [388, 64] // file icon delete
    ];

    private readonly Encoding _shiftJis = Encoding.GetEncoding("shift-jis");

    public IconSys ReadData(byte[] bytes)
    {
        var magic = Encoding.ASCII.GetString(UnpackData(bytes, 0));
        if (magic != "PS2D")
        {
            throw new Exception("Sony icon.sys File Not Formatted");
        }

        var fullTitle = ByteUtilities.ZeroTerminate(UnpackData(bytes, 14));
        var titleOffset = BitConverter.ToUInt16(UnpackData(bytes, 1));
        var splitAt = titleOffset < fullTitle.Length ? titleOffset : fullTitle.Length;

        var shiftJisTitle1 = _shiftJis.GetString(fullTitle[..splitAt]);
        var shiftJisTitle2 = splitAt < fullTitle.Length ? _shiftJis.GetString(fullTitle[splitAt..]) : "";

        var normalizedTitle1 = ShiftJisUtilities.NormalizeText(shiftJisTitle1);
        var normalizedTitle2 = ShiftJisUtilities.NormalizeText(shiftJisTitle2);

        var iconSys = new IconSys
        {
            Magic = magic,
            Title = (normalizedTitle1 ?? shiftJisTitle1, normalizedTitle2 ?? shiftJisTitle2),
            TitleInJis = (shiftJisTitle1, shiftJisTitle2),
            BackgroundTransparency = BitConverter.ToUInt32(UnpackData(bytes, 2)),
            BackgroundColors = [
                BitConverter.ToUInt32(UnpackData(bytes, 3)),
                BitConverter.ToUInt32(UnpackData(bytes, 4)),
                BitConverter.ToUInt32(UnpackData(bytes, 5)),
                BitConverter.ToUInt32(UnpackData(bytes, 6))
            ],
            LightDirections = [
                BitConverter.ToUInt32(UnpackData(bytes, 7)),
                BitConverter.ToUInt32(UnpackData(bytes, 8)),
                BitConverter.ToUInt32(UnpackData(bytes, 9))
            ],
            LightColors = [
                BitConverter.ToUInt32(UnpackData(bytes, 10)),
                BitConverter.ToUInt32(UnpackData(bytes, 11)),
                BitConverter.ToUInt32(UnpackData(bytes, 12))
            ],
            AmbientLightColor = BitConverter.ToUInt32(UnpackData(bytes, 13)),
            IconFileNormal = ByteUtilities.ZeroTerminate(UnpackData(bytes, 15)).ToArray(),
            IconFileCopy = ByteUtilities.ZeroTerminate(UnpackData(bytes, 16)).ToArray(),
            IconFileDelete = ByteUtilities.ZeroTerminate(UnpackData(bytes, 17)).ToArray(),
        };

        return iconSys;
    }

    private static Span<byte> UnpackData(byte[] bytes, int position)
    {
        return bytes.AsSpan(dataMap[position][0], dataMap[position][1]);
    }
}
