namespace MyMc.Models;

public class IconSys
{
    public const string FileName = "icon.sys";

    public string Magic { get; set; } = "";
    public (string Part1, string Part2) Title { get; set; } = ("", "");
    public (string Part1, string Part2) TitleInJis { get; set; } = ("", "");
    public uint BackgroundTransparency { get; set; }
    public uint[] BackgroundColors { get; set; } = new uint[16];
    public double[] LightDirection1 { get; set; } = new double[4];
    public double[] LightDirection2 { get; set; } = new double[4];
    public double[] LightDirection3 { get; set; } = new double[4];
    public double[] LightColor1 { get; set; } = new double[4];
    public double[] LightColor2 { get; set; } = new double[4];
    public double[] LightColor3 { get; set; } = new double[4];
    public double[] AmbientLightColor { get; set; } = new double[4];
    public byte[] IconFileNormal { get; set; } = [];
    public byte[] IconFileCopy { get; set; } = [];
    public byte[] IconFileDelete { get; set; } = [];
}
