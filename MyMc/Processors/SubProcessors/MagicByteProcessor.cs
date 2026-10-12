using System.Text;

namespace MyMc.Processors;

public class MagicByteProcessor
{
    public int StartIndex => 0;

    public int EndIndex => 27;

    private readonly byte[] buffer = new byte[28];

    public void Init()
    {
        Console.WriteLine("Begin verifying the memory card format...");
    }

    public void ProcessByte(byte b, int index)
    {
        buffer[index] = b;
    }

    public void Verify()
    {
        var decodedText = Encoding.ASCII.GetString(buffer);
        if (decodedText != "Sony PS2 Memory Card Format ")
        {
            throw new Exception("Sony Memory Card Not Formatted");
        }

        Console.WriteLine("Successfully verified the memory card format.");
    }
}
