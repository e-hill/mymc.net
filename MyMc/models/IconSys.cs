namespace MyMc.Models;

public class IconSys
{
    public const string FileName = "icon.sys";

    public string Magic { get; set; } = "";
    public (string Part1, string Part2) Title { get; set; } = ("", "");
    public (string Part1, string Part2) TitleInJis { get; set; } = ("", "");
    public uint BackgroundTransparency { get; set; }
    public uint[] BackgroundColors { get; set; } = new uint[4];
    public uint[] LightDirections { get; set; } = new uint[3];
    public uint[] LightColors { get; set; } = new uint[3];
    public uint AmbientLightColor { get; set; }
    public byte[] IconFileNormal { get; set; } = [];
    public byte[] IconFileCopy { get; set; } = [];
    public byte[] IconFileDelete { get; set; } = [];
}
