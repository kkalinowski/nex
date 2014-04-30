using lib12.WPF.Core;
using nex.DirectoryView;

namespace nex.Dialogs.DirectorySynchronizeDialog
{
    public class DirectoryComparison : NotifyingObject
    {
        private bool synchronize;

        public IDirectoryViewItem Left { get; private set; }
        public IDirectoryViewItem Right { get; private set; }
        public DirectoryComparisonResult Result { get; private set; }
        public bool Synchronize
        {
            get
            {
                return synchronize;
            }
            set
            {
                synchronize = value;
                OnPropertyChanged("Synchronize");
            }
        }

        public DirectoryComparison(IDirectoryViewItem left, IDirectoryViewItem right, DirectoryComparisonResult result)
        {
            Left = left;
            Right = right;
            Result = result;
            synchronize = true;
        }
    }
}