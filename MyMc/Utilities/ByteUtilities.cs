namespace MyMc.Utilities;

public static class ByteUtilities
{
    public static Span<byte> ZeroTerminate(Span<byte> bytes)
    {
        var nullTerminator = bytes.IndexOf(byte.MinValue);
        return nullTerminator == -1 ? bytes : bytes[0..nullTerminator];
    }
}
