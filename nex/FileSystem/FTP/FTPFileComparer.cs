using System.Collections.Generic;
using nex.DirectoryView;

namespace nex.FileSystem.FTP
{
    public class FTPFileComparer : IComparer<IDirectoryViewItem>
    {
        int IComparer<IDirectoryViewItem>.Compare(IDirectoryViewItem x, IDirectoryViewItem y)
        {
            if (x.IsMoveUp == y.IsMoveUp)
            {
                if (x.IsDirectory == y.IsDirectory)//directory second
                    return string.Compare(x.Name, y.Name, false);//than name
                else if (x.IsDirectory)
                    return -1;
                else
                    return 1;
            }
            else if (x.IsMoveUp)
                return -1;
            else
                return 1;
        }
    }
}