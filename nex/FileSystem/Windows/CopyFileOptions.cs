using System;

namespace nex.FileSystem.Windows
{
    [Flags]
    public enum CopyFileOptions
    {
        None = 0x0,
        FailIfDestinationExists = 0x1,
        Restartable = 0x2,
        AllowDecryptedDestination = 0x8,
        All = FailIfDestinationExists | Restartable | AllowDecryptedDestination
    }
}