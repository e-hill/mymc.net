namespace MyMc.Extensions;

public static class FileStreamExtensions
{
    extension(FileStream fs)
    {
        public Span<byte> ReadWord()
        {
            Span<byte> buffer = new byte[ByteLengths.Word];
            fs.ReadExactly(buffer);
            return buffer;
        }

        public Span<byte> ReadHalf()
        {
            Span<byte> buffer = new byte[ByteLengths.Half];
            fs.ReadExactly(buffer);
            return buffer;
        }
    }
}
