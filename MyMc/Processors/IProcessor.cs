namespace MyMc.Processors;

public interface IProcessor
{
    public int StartIndex { get; }
    public int EndIndex { get; }
    void Init();
    void ProcessByte(byte b, int index);
    void Verify();
}
