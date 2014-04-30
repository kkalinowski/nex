using System;

namespace nex.FileSystem.Windows
{
    [Flags]
    public enum MoveFileOptions : int
    {
        None = 0x0,
        ReplaceExisting = 0x00000001,
        CopyAllowed = 0x00000002,
        DelayUntilReboot = 0x00000004,
        WriteThrough = 0x00000008,
        CreateHardlink = 0x00000010,
        FailIfNotTrackable = 0x00000020
    }
}