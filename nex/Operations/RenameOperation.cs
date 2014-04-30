using System;
using nex.DirectoryView;
using nex.FileSystem;
using nex.Utilities;

namespace nex.Operations
{
    /// <summary>
    /// Represents one file rename operation
    /// </summary>
    [Serializable]
    public class RenameOperation : OperationBase
    {
        #region Props
        public override bool CanUndo
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Get or pset object to rename
        /// </summary>
        public IDirectoryViewItem ObjectToRename { get; private set; }

        /// <summary>
        /// Get or pset old name
        /// </summary>
        public string OldName { get; private set; }

        /// <summary>
        /// Get or pset new name
        /// </summary>
        public string NewName { get; private set; }

        public string NewPath { get; private set; }
        public string OldPath { get; private set; }
        #endregion

        #region ctor
        public RenameOperation(IDirectoryViewItem objectToRename, string newName, FileSystemBase fileSystem)
            : base(fileSystem)
        {
            ObjectToRename = objectToRename;
            OldName = ObjectToRename.Name;
            OldPath = ObjectToRename.FullName;
            NewName = newName;
            NewPath = PathExt.Combine(PathExt.GetDirectoryName(ObjectToRename.FullName, ObjectToRename.IsWindowsFile), NewName, ObjectToRename.IsWindowsFile);
            OperationName = "Zmiana nazwy z " + OldName + " na " + NewName;
        }
        #endregion

        #region Logic
        protected override void Execute()
        {
            if (FileSystem.CheckIfObjectExist(NewPath))
            {
                MessageService.ShowError("Obiekt o nazwie " + NewName + " już isnieje. Nie mogę przeprowadzić operacji.");
                IsCanceled = true;
            }
            else
            {
                FileSystem.Rename(ObjectToRename.FullName, NewPath, ObjectToRename.IsDirectory);
            }

            OnFinished();
        }

        public override void Undo()
        {
            if (FileSystem.CheckIfObjectExist(OldPath))
            {
                MessageService.ShowError("Obiekt o nazwie " + OldName + " już isnieje. Nie mogę cofnąć operacji.");
            }
            else
            {
                FileSystem.Rename(NewPath, OldPath, ObjectToRename.IsDirectory);
            }
        }
        #endregion
    }
}