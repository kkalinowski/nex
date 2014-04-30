using System.IO;

namespace nex.FileSystem.Windows
{
    public delegate CopyFileCallbackAction CopyFileCallback(FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred);
}