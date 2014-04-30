using lib12.WPF.Core;
using nex.DirectoryView;

namespace nex.Dialogs.MultiRenameDialog
{
    public class MultiRenameItem : NotifyingObject
    {
        private string newName;

        public IDirectoryViewItem Item { get; set; }
        public string NewName
        {
            get
            {
                return newName;
            }
            set
            {
                newName = value;
                OnPropertyChanged("NewName");
            }
        }
        public string OldName { get; private set; }

        public MultiRenameItem(IDirectoryViewItem item)
        {
            Item = item;
            OldName = item.Name;
        }
    }
}