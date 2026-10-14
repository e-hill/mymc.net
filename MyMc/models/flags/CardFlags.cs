namespace MyMc.Models.Flags;

[Flags]
public enum CardFlags
{
    CF_USE_ECC = 0x01,
    CF_BAD_BLOCK = 0x08,
    CF_ERASE_ZEROES = 0x10
}
