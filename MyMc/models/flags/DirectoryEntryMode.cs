namespace MyMc.Models.Flags;

[Flags]
public enum DirectoryEntryMode : int
{
    INVALID = -1,
    DF_READ = 0x0001, // read permission
    DF_WRITE = 0x0002, // werite permission
    DF_EXECUTE = 0x0004, // execute permission
    DF_PROTECTED = 0x0008, // directory is copy protected
    DF_FILE = 0x0010, // regular file
    DF_DIRECTORY = 0x0020, // directory
    DF_CREATE = 0x0040, // used internally to create directories
    DF_COPIED = 0x0080, // copied?
    DF_0100 = 0x0100,
    O_CREAT = 0x0200, // used to create files
    DF_0400 = 0x0400, // set when files and directories are created
    DF_POCKETSTN = 0x0800, // pocketstation application save file
    DF_PSX = 0x1000, // playstation save file
    DF_HIDDEN = 0x2000, // file is hidden
    DF_4000 = 0x4000,
    DF_EXISTS = 0x8000 // this entry is in use
}
