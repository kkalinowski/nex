using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using nex.DirectoryView;
using nex.FileSystem;
using nex.FileSystem.Windows;
using nex.HistoryLogic;

namespace nex.Controls.DirectoryViews
{
    /// <summary>
    /// Interaction logic for PreviewView.xaml
    /// </summary>
    public partial class PreviewView : UserControl, IDirectoryView, INotifyPropertyChanged
    {
        #region Events Declaration
        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when value of property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>   
        /// Raise a PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of property, that value have been changed</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ItemExecuted
        public event EventHandler ItemExecuted;

        protected void OnItemExecuted(IDirectoryViewItem item)
        {
            var handler = ItemExecuted;
            if (handler != null)
                handler(item, null);
        }

        #endregion
        #endregion

        #region Fields
        private bool dirChanged = true;//determines if directory has changed and is needed to reload
        #endregion

        #region Props
        public string DisplayPath
        {
            get
            {
                return FileSystem.FullPath;
            }
        }

        public bool IsMoveUpSelected
        {
            get
            {
                return ((IDirectoryViewItem)Items[0]).IsMoveUp && SelectedItems.Contains(Items[0]);
            }
        }

        public bool IsActive
        {
            get
            {
                return lbContent.IsKeyboardFocusWithin;
            }
        }

        public bool IsOneFileSelected
        {
            get
            {
                return SelectedItemsCount == 1 && !IsMoveUpSelected && !SelectedItem.IsDirectory;
            }
        }

        public ItemCollection Items
        {
            get
            {
                return lbContent.Items;
            }
        }

        public IDirectoryViewItem SelectedItem
        {
            get
            {
                return (IDirectoryViewItem)lbContent.SelectedItem;
            }
        }

        public IList SelectedItems
        {
            get
            {
                return lbContent.SelectedItems;
            }
        }

        public int SelectedItemsCount
        {
            get
            {
                return lbContent.SelectedItems.Count;
            }
        }

        public FileSystemBase FileSystem { get; private set; }

        public History<IDirectoryViewItem> History { get; set; }

        public string DirectoryName
        {
            get
            {
                return FileSystem.DirectoryName;
            }
        }
        #endregion

        #region ctor
        public PreviewView()
        {
            InitializeComponent();
            FileSystem = new WindowsFileSystem();
            History = new History<IDirectoryViewItem>();

            Binding bind = new Binding("Items");
            bind.Source = FileSystem;
            lbContent.SetBinding(ListBox.ItemsSourceProperty, bind);

            lbContent.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        #endregion

        #region Events Support
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            ItemContainerGenerator generator = (ItemContainerGenerator)sender;
            if (generator.Status == GeneratorStatus.ContainersGenerated && dirChanged)
            {
                SetFocusOnContent();
                dirChanged = false;
            }
        }

        private void PreviewView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem clicked = (ListBoxItem)sender;
            IDirectoryViewItem item = (IDirectoryViewItem)clicked.Content;
            OnItemExecuted(item);
        }

        #endregion

        #region Methods
        public void LoadDir(string dir, bool saveInHistory)
        {
            FileSystem.LoadDirectory(dir);
            dirChanged = true;

            //save in history
            if (saveInHistory)
            {
                HistoryGlobal.AddItem(FileSystem.CurrentPlace);
                History.AddItem(FileSystem.CurrentPlace, false);
            }
        }

        public void LoadSelectedDir()
        {
            LoadDir(SelectedItem.FullName, true);
        }

        public void MoveBack()
        {
            LoadDir(History.MoveBack().FullName, false);
        }

        public void MoveForward()
        {
            LoadDir(History.MoveForward().FullName, false);
        }

        public void MoveUp()
        {
            if (!!FileSystem.IsRootPath(FileSystem.CurrentPlace.FullName))
                LoadDir(PathExt.GetDirectoryName(DisplayPath, FileSystem.IsWindowsFileSystem), true);
        }

        public void SelectAll()
        {
            lbContent.SelectAll();
            if (Items.Count > 1 && ((IDirectoryViewItem)Items[0]).IsMoveUp)
                lbContent.SelectedItems.Remove(Items[0]);
        }

        public void UnselectAll()
        {
            lbContent.UnselectAll();
        }

        public void SetFocusOnContent()
        {
            //if nothing is selected
            if (SelectedItem == null)
            {
                //focus on 1st item in most cases, on 0th if it is drive root or dir is empty
                lbContent.SelectedIndex = Items.Count == 1 || PathExt.IsDriveRoot(DisplayPath) ? 0 : 1;
                //lbContent.ScrollIntoView(SelectedItem); - bug when calling copy on multiple objects not in the view
            }

            ListBoxItem item = lbContent.ItemContainerGenerator.ContainerFromItem(SelectedItem) as ListBoxItem;
            if (item != null)
                item.Focus();//If item==null AfterCtorInit() call is needed
        }

        public void ChangeFileSystem(FileSystem.FileSystemBase fileSystem)
        {
            FileSystem = fileSystem;

            //set up binding
            Binding bind = new Binding("Items");
            bind.Source = FileSystem;
            lbContent.SetBinding(ListBox.ItemsSourceProperty, bind);

            //save in history
            HistoryGlobal.AddItem(FileSystem.CurrentPlace);
            History.AddItem(FileSystem.CurrentPlace, false);

            dirChanged = true;
            SetFocusOnContent();
            OnPropertyChanged("FileSystem");
        }
        #endregion
    }
}