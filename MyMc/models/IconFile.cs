namespace MyMc.Models;

public class IconFile
{
    public const double FixedPointFactor = 4096.0;

    public uint VertexCount { get; set; }

    /// Number of morph-target animation shapes (frames).
    public uint AnimationShapes { get; set; }

    /// Per-shape vertex positions: [animationShapes] lists, each [vertexCount×3] floats (x,y,z).
    public double[][] Positions { get; set; } = [];

    /// Static vertex normals: [vertexCount×3] floats (nx,ny,nz).
    public double[] Normals { get; set; } = [];

    /// Static UV coordinates: [vertexCount×2] floats (u,v).
    public double[] Uvs { get; set; } = [];

    /// Static per-vertex colors: [vertexCount×4] bytes (RGBA).
    public byte[] VertexColors { get; set; } = [];

    // Decoded texture: 128×128 pixels, RGB (3 bytes/pixel), or null if absent.
    public byte[]? Texture { get; set; }

    public uint FrameLength { get; set; }
    public float AnimationSpeed { get; set; }
    public uint PlayOffset { get; set; }
    public uint FrameCount { get; set; }
}
