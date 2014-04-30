using System;
using System.Collections;
using System.Windows.Controls;
using nex.DirectoryView;
using nex.FileSystem;
using nex.HistoryLogic;

namespace nex.Controls.DirectoryViews
{
    public interface IDirectoryView
    {
        #region Events
        event EventHandler ItemExecuted;

        #endregion

        #region Props

        string DisplayPath { get; }
        bool IsMoveUpSelected { get; }
        bool IsOneFileSelected { get; }
        ItemCollection Items { get; }
        IDirectoryViewItem SelectedItem { get; }
        IList SelectedItems { get; }
        int SelectedItemsCount { get; }
        FileSystemBase FileSystem { get; }
        History<IDirectoryViewItem> History { get; set; }
        string DirectoryName { get; }
        bool IsActive { get; }

        #endregion

        #region Methods

        void LoadDir(string dir, bool saveInHistory);

        void LoadSelectedDir();

        void MoveBack();

        void MoveForward();

        void MoveUp();

        void SelectAll();

        void UnselectAll();

        /// <summary>
        /// Set focous on item no. zero or one, depend of the folder content
        /// Use it only to change active view!!!
        /// </summary>
        void SetFocusOnContent();

        /// <summary>
        /// Changes the file system used by view
        /// </summary>
        /// <param name="fileSystem">New file system</param>
        void ChangeFileSystem(FileSystemBase fileSystem);
        #endregion
    }
}