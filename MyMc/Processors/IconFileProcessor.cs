using MyMc.Models;

namespace MyMc.Processors;

public class IconFileProcessor
{
    public IconFile? ReadData(byte[] bytes)
    {
        var magic = BitConverter.ToUInt32(bytes.AsSpan(0, 4));
        var animationShapes = BitConverter.ToUInt32(bytes.AsSpan(4, 4));
        var textureType = BitConverter.ToUInt32(bytes.AsSpan(8, 4));
        var vertexCount = BitConverter.ToUInt32(bytes.AsSpan(16, 4));

        if (magic != 0x010000)
        {
            throw new Exception("Sony *.ico File Not Formatted");
        }

        if (animationShapes == 0 || vertexCount == 0) return null;
        if (vertexCount % 3 != 0) return null;

        var offset = 20;

        var scale = 1.0 / IconFile.FixedPointFactor;
        var needed = vertexCount * (animationShapes * 8 + 8 + 4 + 4);
        if (offset + needed > bytes.Length) return null;

        var posArrays = new double[animationShapes][];

        for (int i = 0; i < animationShapes; i++)
        {
            posArrays[i] = new double[vertexCount * 3];
        }

        var normalsBuf = new double[vertexCount * 3];
        var uvsBuf = new double[vertexCount * 2];
        var colorsBuf = new byte[vertexCount * 4];

        // --- Geometry (interleaved per vertex) ---
        // Per vertex i:
        //   animShapes × (int16 x,y,z + uint16 pad) = animShapes × 8 bytes  [positions]
        //   int16 nx,ny,nz + uint16 pad              = 8 bytes               [normal, static]
        //   int16 u,v                                = 4 bytes               [uv, static]
        //   uint8 r,g,b,a                            = 4 bytes               [color, static]

        for (int v = 0; v < vertexCount; v++)
        {
            for (int s = 0; s < animationShapes; s++)
            {
                var b = v * 3;
                posArrays[s][b] = BitConverter.ToInt16(bytes.AsSpan(offset, 2)) * scale;
                posArrays[s][b + 1] = BitConverter.ToInt16(bytes.AsSpan(offset + 2, 2)) * scale;
                posArrays[s][b + 2] = BitConverter.ToInt16(bytes.AsSpan(offset + 4, 2)) * scale;
                offset += 8;
            }


            var nb = v * 3;
            normalsBuf[nb] = BitConverter.ToInt16(bytes.AsSpan(offset, 2)) * scale;
            normalsBuf[nb + 1] = BitConverter.ToInt16(bytes.AsSpan(offset + 2, 2)) * scale;
            normalsBuf[nb + 2] = BitConverter.ToInt16(bytes.AsSpan(offset + 4, 2)) * scale;
            offset += 8;

            var ub = v * 2;
            uvsBuf[ub] = BitConverter.ToInt16(bytes.AsSpan(offset, 2)) * scale;
            uvsBuf[ub + 1] = BitConverter.ToInt16(bytes.AsSpan(offset + 2, 2)) * scale;
            offset += 4;

            var cb = v * 4;
            colorsBuf[cb] = bytes[offset];
            colorsBuf[cb + 1] = bytes[offset + 1];
            colorsBuf[cb + 2] = bytes[offset + 2];
            colorsBuf[cb + 3] = bytes[offset + 3];
            offset += 4;
        }

        // --- Animation header (uint32 + uint32 + float32 + uint32 + uint32 = 20 bytes) ---
        if (offset + 20 > bytes.Length) return null;

        var animMagic = BitConverter.ToUInt32(bytes.AsSpan(offset, 4));
        var frameLength = BitConverter.ToUInt32(bytes.AsSpan(offset + 4, 4));
        var animSpeed = BitConverter.ToSingle(bytes.AsSpan(offset + 8, 4));
        var playOffset = BitConverter.ToUInt32(bytes.AsSpan(offset + 12, 4));
        var frameCount = BitConverter.ToUInt32(bytes.AsSpan(offset + 16, 4));
        offset += 20;

        if (animMagic != 0x01) return null;

        // Skip frame key data (frame_count frames, each with 4×uint32 header + (key_count-1)×2×float32)
        for (int f = 0; f < frameCount; f++)
        {
            if (offset + 16 > bytes.Length) return null;
            var keyCount = BitConverter.ToUInt32(bytes.AsSpan(offset + 4, 4));
            offset += 16; // 4 × uint32 frame_data
            offset += ((int)keyCount - 1) * 8; // (key_count-1) × 2×float32
        }

        // --- Texture ---
        byte[]? texture = null;
        var hasTexture = (textureType & 0x04) != 0;
        var isCompressed = (textureType & 0x08) != 0;

        if (hasTexture && offset < bytes.Length)
        {
            var raw = isCompressed
                ? DecodeRle(bytes, offset)
                : bytes.AsSpan(new Range(offset, Math.Clamp(offset + 128 * 128 * 2, 0, bytes.Length)));
            if (raw != Span<byte>.Empty) texture = DecodeA1B5G5R5(raw);
        }

        return new IconFile
        {
            VertexCount = vertexCount,
            AnimationShapes = animationShapes,
            Positions = posArrays,
            Normals = normalsBuf,
            Uvs = uvsBuf,
            VertexColors = colorsBuf,
            Texture = texture,
            FrameLength = frameLength,
            AnimationSpeed = animSpeed,
            PlayOffset = playOffset,
            FrameCount = frameCount
        };
    }

    // ---------------------------------------------------------------------------
    // Texture helpers
    // ---------------------------------------------------------------------------

    /// Decode RLE-compressed texture to raw A1B5G5R5 bytes.
    private static byte[]? DecodeRle(byte[] bytes, int offset)
    {
        if (offset + 4 > bytes.Length) return null;
        var compressedSize = BitConverter.ToUInt32(bytes.AsSpan(offset, 4));
        offset += 4;
        List<byte> result = [];
        int rleOffset = 0;
        while (rleOffset < compressedSize)
        {
            if (offset + rleOffset + 2 > bytes.Length) break;
            var code = BitConverter.ToUInt16(bytes.AsSpan(offset + rleOffset, 2));
            rleOffset += 2;
            if ((code & 0x8000) != 0)
            {
                var count = 0x8000 - (code ^ 0x8000);
                var end = Math.Clamp(offset + rleOffset + count * 2, 0, bytes.Length);
                result.AddRange(bytes.AsSpan(new Range(offset + rleOffset, end)));
                rleOffset += count * 2;
            }
            else
            {
                if (code > 0)
                {
                    if (offset + rleOffset + 2 > bytes.Length) break;
                    var pixel = bytes.AsSpan(offset + rleOffset, 2);
                    for (int t = 0; t < code; t++) result.AddRange(pixel);
                    rleOffset += 2;
                }
            }
        }
        return [.. result];
    }


    /// Decode A1B5G5R5 (16-bit LE) to RGB (3 bytes/pixel).
    private static byte[] DecodeA1B5G5R5(Span<byte> raw)
    {
        var pixelCount = raw.Length / 2;
        var result = new byte[pixelCount * 3];
        for (int i = 0; i < pixelCount; i++)
        {
            var p = BitConverter.ToUInt16(raw.Slice(i * 2, 2));
            result[i * 3] = (byte)((p & 0x1F) << 3);             // R
            result[i * 3 + 1] = (byte)(((p >> 5) & 0x1F) << 3);  // G
            result[i * 3 + 2] = (byte)(((p >> 10) & 0x1F) << 3); // B
        }
        return result;
    }
}
