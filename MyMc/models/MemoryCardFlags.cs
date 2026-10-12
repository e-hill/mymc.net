namespace MyMc.Models;

public enum MemoryCardFlags
{
    CF_USE_ECC = 0x01, // Card supports ECC.
    CF_BAD_BLOCK = 0x08, // Card may have bad blocks.
    CF_ERASE_ZEROES = 0x10 // Erased blocks have all bits set to zero.
}