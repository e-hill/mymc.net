namespace MyMc.Models;

public class IconFile
{
    public int VertexCount { get; set; }

    /// Number of morph-target animation shapes (frames).
    public int AnimationShapes { get; set; }

    /// Per-shape vertex positions: [animationShapes] lists, each [vertexCount×3] floats (x,y,z).
    public float[][] Positions { get; set; } = [];

    /// Static vertex normals: [vertexCount×3] floats (nx,ny,nz).
    public float[] Normals { get; set; } = [];

    /// Static UV coordinates: [vertexCount×2] floats (u,v).
    public float[] Uvs { get; set; } = [];

    /// Static per-vertex colors: [vertexCount×4] bytes (RGBA).
    public byte[] VertexColors { get; set; } = [];

    // Decoded texture: 128×128 pixels, RGB (3 bytes/pixel), or null if absent.
    public byte[]? Texture { get; set; }

    public int FrameLength { get; set; }
    public double AnimationSpeed { get; set; }
    public int PlayOffset { get; set; }
    public int FrameCount { get; set; }
}
