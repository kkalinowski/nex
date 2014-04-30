using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using lib12.WPF.Core;
using nex.DirectoryView;

namespace nex.FileSystem
{
    /// <summary>
    /// Base class for file systems
    /// </summary>
    [Serializable]
    public abstract class FileSystemBase : NotifyingObject, IDisposable
    {
        #region Fields
        private IDirectoryViewItem currentPlace;
        #endregion

        #region Props
        /// <summary>
        /// Get or set current file system place
        /// </summary>
        public IDirectoryViewItem CurrentPlace
        {
            get
            {
                return currentPlace;
            }
            set
            {
                currentPlace = value;
                OnPropertyChanged("CurrentPlace");
                OnPropertyChanged("DirectoryName");
                OnPropertyChanged("FullPath");
            }
        }

        /// <summary>
        /// Get or set full path to directory
        /// </summary>
        public string FullPath
        {
            get
            {
                return CurrentPlace.FullName;
            }
        }

        /// <summary>
        /// Get current directory name
        /// </summary>
        public string DirectoryName
        {
            get
            {
                return CurrentPlace.Name;
            }
        }

        [NonSerialized]
        private List<IDirectoryViewItem> items;
        /// <summary>
        /// Get current place items
        /// </summary>
        public List<IDirectoryViewItem> Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Get FileSystemRoot
        /// </summary>
        public abstract FileSystemRoot Root { get; protected set; }

        public abstract bool IsWindowsFileSystem { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Get the content without changing current place
        /// </summary>
        /// <param name="path">Path to directory</param>
        /// <returns>Content of directory</returns>
        public abstract IEnumerable<IDirectoryViewItem> GetDirectoryContent(string path);

        /// <summary>
        /// Load content of directory into Items
        /// </summary>
        /// <param name="path">Path to directory</param>
        public abstract void LoadDirectory(string path);

        public virtual void LoadDefaultDirectory()
        {
            LoadDirectory(Root.Path);
        }

        public bool IsRootPath(string path)
        {
            return path == Root.Path || path == Root.Path + "/";
        }

        public abstract bool CheckIfObjectExist(string path);

        public abstract FileSystemBase GetCopy();
        #endregion

        #region Operations
        public abstract IDirectoryViewItem CreateNewDirectory(string path);

        public abstract void Delete(IDirectoryViewItem toDelete);

        public abstract void Rename(string from, string to, bool isDirectory);
        #endregion

        #region ctor
        protected FileSystemBase()
        {
            items = new List<IDirectoryViewItem>();
        } 
        #endregion

        #region Serialization
        [OnDeserialized]
        private void InitAfterSerialization(StreamingContext context)
        {
            items = new List<IDirectoryViewItem>();
        } 
        #endregion

        #region Dispose
        public abstract void Dispose();
        #endregion
    }
}