using System;
using System.IO;
using System.Windows;
using nex.DirectoryView;
using nex.FileSystem;

namespace nex.Operations
{
    /// <summary>
    /// Represents the creation of new folder operation
    /// </summary>
    [Serializable]
    public class NewFolderOperation : OperationBase
    {
        #region Props
        /// <summary>
        /// Get or set new folder 
        /// </summary>
        public string FolderPath { get; private set; }

        /// <summary>
        /// Get or set new folder object
        /// </summary>
        public IDirectoryViewItem FolderObject { get; private set; }

        public override bool CanUndo
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region ctor
        public NewFolderOperation(string folderPath, FileSystemBase fileSystem)
            : base(fileSystem)
        {
            FolderPath = folderPath;
            OperationName = "Tworzenie folderu " + Path.GetFileName(FolderPath);
        }
        #endregion

        #region Logic
        /// <summary>
        /// Executes the creation of new folder operation
        /// </summary>
        protected override void Execute()
        {
            if (FileSystem.CheckIfObjectExist(FolderPath))
            {
                MessageBox.Show("Obiekt o nazwie " + Path.GetFileName(FolderPath) + " już isnieje. Nie mogę przeprowadzić operacji.");
                IsCanceled = true;
            }
            else
            {
                FolderObject = FileSystem.CreateNewDirectory(FolderPath);
            }

            if (!IsCanceled)
                OnFinished();
        }

        public override void Undo()
        {
            //OPT: Check if folder has content
            FileSystem.Delete(FolderObject);
        } 
        #endregion
    }
}