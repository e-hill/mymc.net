using System.Runtime.InteropServices;
using Raylib_cs;

namespace Icon;

public class BuildIcon
{
    public unsafe static Mesh BuildIconMesh(int vertexCount, double[] initialPositions, double[] normals, double[] uvs, byte[] vertexColors)
    {
        var mesh = new Mesh
        {
            VertexCount = vertexCount,
            TriangleCount = vertexCount / 3
        };

        int posFloats = vertexCount * 3;
        int normFloats = vertexCount * 3;
        int uvFloats = vertexCount * 2;
        int colorBytes = vertexCount * 4;

        mesh.Vertices = (float*)NativeMemory.Alloc((nuint)(posFloats * sizeof(float)));
        mesh.Normals = (float*)NativeMemory.Alloc((nuint)(normFloats * sizeof(float)));
        mesh.TexCoords = (float*)NativeMemory.Alloc((nuint)(uvFloats * sizeof(float)));
        mesh.Colors = (byte*)NativeMemory.Alloc((nuint)colorBytes);

        CopyDoublesToFloats(initialPositions, mesh.Vertices, posFloats);
        CopyDoublesToFloats(normals, mesh.Normals, normFloats);
        CopyDoublesToFloats(uvs, mesh.TexCoords, uvFloats);

        int copyLen = Math.Min(colorBytes, vertexColors.Length);
        fixed (byte* srcCol = vertexColors)
        {
            NativeMemory.Copy(srcCol, mesh.Colors, (nuint)copyLen);
        }

        Raylib.UploadMesh(ref mesh, dynamic: true);
        return mesh;
    }

    public static unsafe void UpdateFramePositions(Mesh mesh, int vertexCount, double[] framePositions)
    {
        int posFloats = vertexCount * 3;
        CopyDoublesToFloats(framePositions, mesh.Vertices, posFloats);
        Raylib.UpdateMeshBuffer(mesh, 0, mesh.Vertices, posFloats * sizeof(float), 0);
    }

    public static unsafe Texture2D BuildTexture(byte[]? textureBytes, int width = 128, int height = 128, bool hasAlpha = false)
    {
        if (textureBytes == null || textureBytes.Length == 0)
            return default;

        fixed (byte* ptr = textureBytes)
        {
            var image = new Image
            {
                Data = ptr,
                Width = width,
                Height = height,
                Mipmaps = 1,
                Format = hasAlpha ? PixelFormat.UncompressedR8G8B8A8 : PixelFormat.UncompressedR8G8B8
            };
            return Raylib.LoadTextureFromImage(image);
        }
    }

    private static unsafe void CopyDoublesToFloats(double[] src, float* dst, int count)
    {
        int n = Math.Min(count, src.Length);
        for (int i = 0; i < n; i++)
            dst[i] = (float)src[i];

        for (int i = n; i < count; i++)
            dst[i] = 0f;
    }
}
