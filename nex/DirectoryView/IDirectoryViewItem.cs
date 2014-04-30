using System;
using System.IO;
using System.Windows.Media;
using nex.FileSystem;
using nex.Operations;

namespace nex.DirectoryView
{
    /// <summary>
    /// Interface describing abstract file system object
    /// </summary>
    public interface IDirectoryViewItem
    {
        #region Props
        FileAttributes Attributes { get; }
        ImageSource FileIcon { get; }
        bool Exists { get; }
        DateTime LastModifiedTime { get; set; }
        FileSize Size { get; }
        string Name { get; }
        string NameWithoutExt { get; }
        string Ext { get; }
        string FullName { get; }
        bool IsDirectory { get; }
        bool IsMoveUp { get; }
        bool? IsMatchingCriteria { get; set; }
        bool IsWindowsFile { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Copy object to another place
        /// </summary>
        /// <param name="destDir">Destination directory</param>
        /// <param name="operation">Operation during which object is copied. Needed for reporting changes</param>
        void CopyTo(string destDir, CopyOperation operation);

        /// <summary>
        /// Move object to another place
        /// </summary>
        /// <param name="destDir">Destination directory</param>
        /// <param name="operation">Operation during which object is moved. Needed for reporting changes</param>
        void MoveTo(string destDir, MoveOperation operation);

        void CountFolderSize();

        /// <summary>
        /// Creates files system correct for this item
        /// </summary>
        /// <returns>New file system which would containt this IDirectoryViewItem</returns>
        FileSystemBase CreateFileSystem();
        #endregion
    }
}