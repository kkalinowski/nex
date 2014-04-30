using System;
using System.IO;
using System.Windows.Media;
using nex.FileSystem;
using nex.Operations;
using nex.Utilities;

namespace nex.DirectoryView
{
    public class MoveUpObject : IDirectoryViewItem
    {
        private static ImageSource icon = null;

        static MoveUpObject()
        {
            //use this code for *.ico files loading 
            //IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri(@"pack://application:,,/Icons/up.ico", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            //icon = ibd.Frames[0];
            //icon = new BitmapImage(new Uri(@"pack://application:,,/Icons/up.png", UriKind.RelativeOrAbsolute));
            icon = Utility.LoadImageSource("UpIcon");
        }

        #region IDirectoryViewItem Members

        public FileAttributes Attributes
        {
            get
            {
                return FileAttributes.Normal;
            }
        }

        public ImageSource FileIcon
        {
            get
            {
                return icon;
            }
        }

        public bool Exists
        {
            get
            {
                return false;
            }
        }

        public DateTime LastModifiedTime
        {
            get
            {
                return DateTime.Now;
            }
            set
            {
            }
        }

        public FileSize Size
        {
            get
            {
                return FileSize.Empty;
            }
        }

        public string Name
        {
            get
            {
                return "W górę";
            }
        }

        public string NameWithoutExt
        {
            get
            {
                return Name;
            }
        }

        public string Ext
        {
            get
            {
                return Name;
            }
        }

        public string FullName
        {
            get
            {
                return "W górę";
            }
        }

        public bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public bool IsMoveUp
        {
            get
            {
                return true;
            }
        }

        public bool IsWindowsFile
        {
            get
            {
                return false;
            }
        }

        public bool? IsMatchingCriteria
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public void CopyTo(string destDir, CopyOperation operation)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(string path, MoveOperation operation)
        {
            throw new NotImplementedException();
        }

        public void CountFolderSize()
        {
            throw new NotImplementedException();
        }

        public FileSystemBase CreateFileSystem()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}