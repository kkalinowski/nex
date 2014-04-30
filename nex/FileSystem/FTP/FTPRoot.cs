using System;
using System.Windows.Media;
using nex.Accounts;
using nex.Utilities;

namespace nex.FileSystem.FTP
{
    [Serializable]
    public class FTPRoot : FileSystemRoot
    {
        #region Fields
        [NonSerialized]
        private static ImageSource icon;
        private readonly string path; 
        #endregion

        #region sctor
        static FTPRoot()
        {
            icon = Utility.LoadImageSource("FTPIcon");
        }
        #endregion

        #region ctor
        public FTPRoot(Account account)
        {
            path = "/";
        } 
        #endregion

        public override string Path
        {
            get
            {
                return path;
            }
        }

        public override string Label
        {
            get
            {
                return "FTP";
            }
        }

        public override double FreePercent
        {
            get
            {
                return 100.0;
            }
        }

        public override ImageSource Icon
        {
            get
            {
                return icon;
            }
        }
    }
}