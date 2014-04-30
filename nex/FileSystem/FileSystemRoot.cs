using System;
using System.Windows.Media;

namespace nex.FileSystem
{
    /// <summary>
    /// Describes root of the file system
    /// </summary>
    [Serializable]
    public abstract class FileSystemRoot
    {
        #region Props
        public abstract string Path { get; }
        public abstract string Label { get; }
        public abstract double FreePercent { get; }
        public virtual double TakenPercent
        {
            get
            {
                return 100.0 - FreePercent;
            }
        }
        public abstract ImageSource Icon { get; }
        #endregion
    }
}