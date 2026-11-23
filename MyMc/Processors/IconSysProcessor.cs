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
        [80, 4], [84, 4], [88, 4], [92, 4],  // light direction 1
        [96, 4], [100, 4], [104, 4], [108, 4],  // light direction 2
        [112, 4], [116, 4], [120, 4], [124, 4],  // light direction 3
        [128, 4], [132, 4], [136, 4], [140, 4], // light color 1
        [144, 4], [148, 4], [152, 4], [156, 4], // light color 2
        [160, 4], [164, 4], [168, 4], [172, 4], // light color 3
        [176, 4], [180, 4], [184, 4], [192, 4], // ambient light color
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

        var fullTitle = ByteUtilities.ZeroTerminate(UnpackData(bytes, 35));
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
            LightDirection1 = [
                BitConverter.ToSingle(UnpackData(bytes, 7)),
                BitConverter.ToSingle(UnpackData(bytes, 8)),
                BitConverter.ToSingle(UnpackData(bytes, 9)),
                BitConverter.ToSingle(UnpackData(bytes, 10))
            ],
            LightDirection2 = [
                BitConverter.ToSingle(UnpackData(bytes, 11)),
                BitConverter.ToSingle(UnpackData(bytes, 12)),
                BitConverter.ToSingle(UnpackData(bytes, 13)),
                BitConverter.ToSingle(UnpackData(bytes, 14))
            ],
            LightDirection3 = [
                BitConverter.ToSingle(UnpackData(bytes, 15)),
                BitConverter.ToSingle(UnpackData(bytes, 16)),
                BitConverter.ToSingle(UnpackData(bytes, 17)),
                BitConverter.ToSingle(UnpackData(bytes, 18))
            ],
            LightColor1 = [
                BitConverter.ToSingle(UnpackData(bytes, 19)),
                BitConverter.ToSingle(UnpackData(bytes, 20)),
                BitConverter.ToSingle(UnpackData(bytes, 21)),
                BitConverter.ToSingle(UnpackData(bytes, 22))
            ],
            LightColor2 = [
                BitConverter.ToSingle(UnpackData(bytes, 23)),
                BitConverter.ToSingle(UnpackData(bytes, 24)),
                BitConverter.ToSingle(UnpackData(bytes, 25)),
                BitConverter.ToSingle(UnpackData(bytes, 26))
            ],
            LightColor3 = [
                BitConverter.ToSingle(UnpackData(bytes, 27)),
                BitConverter.ToSingle(UnpackData(bytes, 28)),
                BitConverter.ToSingle(UnpackData(bytes, 29)),
                BitConverter.ToSingle(UnpackData(bytes, 30))
            ],
            AmbientLightColor = [
                BitConverter.ToSingle(UnpackData(bytes, 31)),
                BitConverter.ToSingle(UnpackData(bytes, 32)),
                BitConverter.ToSingle(UnpackData(bytes, 33)),
                BitConverter.ToSingle(UnpackData(bytes, 34))
            ],
            IconFileNormal = ByteUtilities.ZeroTerminate(UnpackData(bytes, 36)).ToArray(),
            IconFileCopy = ByteUtilities.ZeroTerminate(UnpackData(bytes, 37)).ToArray(),
            IconFileDelete = ByteUtilities.ZeroTerminate(UnpackData(bytes, 38)).ToArray(),
        };

        return iconSys;
    }

    private static Span<byte> UnpackData(byte[] bytes, int position)
    {
        return bytes.AsSpan(dataMap[position][0], dataMap[position][1]);
    }
}
