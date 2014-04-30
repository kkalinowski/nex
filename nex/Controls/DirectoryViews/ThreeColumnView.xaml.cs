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
    /// Interaction logic for ThreeColumnView.xaml
    /// </summary>
    public partial class ThreeColumnView : UserControl, IDirectoryView, INotifyPropertyChanged
    {
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

        public event EventHandler ItemExecuted;

        #region Fields
        private FileSystemBase fileSystem1, fileSystem2, fileSystem3;
        private History<IDirectoryViewItem> history;
        private bool dirChanged = true;
        #endregion

        public ThreeColumnView()
        {
            InitializeComponent();
            fileSystem1 = new WindowsFileSystem();
            fileSystem2 = new WindowsFileSystem();
            fileSystem3 = new WindowsFileSystem();
            history = new History<IDirectoryViewItem>();

            Binding bind2 = new Binding("Items");//bind to second(middle) ListBox
            bind2.Source = FileSystem;
            lbContent2.SetBinding(ListBox.ItemsSourceProperty, bind2);

            lbContent2.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            ItemContainerGenerator generator = (ItemContainerGenerator)sender;
            if (generator.Status == GeneratorStatus.ContainersGenerated && dirChanged)
            {
                SetFocusOnContent();
                dirChanged = false;
            }
        }

        public string DisplayPath
        {
            get
            {
                return fileSystem2.FullPath;
            }
        }

        public bool IsMoveUpSelected
        {
            get
            {
                return ((IDirectoryViewItem)Items[0]).IsMoveUp && SelectedItems.Contains(Items[0]);
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
                return lbContent2.Items;
            }
        }

        public IDirectoryViewItem SelectedItem
        {
            get
            {
                return (IDirectoryViewItem)lbContent2.SelectedItem;
            }
        }

        public IList SelectedItems
        {
            get
            {
                return lbContent2.SelectedItems;
            }
        }

        public int SelectedItemsCount
        {
            get
            {
                return lbContent2.SelectedItems.Count;
            }
        }

        public FileSystemBase FileSystem
        {
            get
            {
                return fileSystem2;
            }
        }

        public History<IDirectoryViewItem> History
        {
            get
            {
                return history;
            }
            set
            {
                history = value;
            }
        }

        public string DirectoryName
        {
            get
            {
                return fileSystem2.DirectoryName;
            }
        }

        public bool IsActive
        {
            get
            {
                return lbContent2.IsKeyboardFocusWithin;
            }
        }

        public void LoadDir(string dir, bool saveInHistory)
        {
            //loading files into second column
            FileSystem.LoadDirectory(dir);
            dirChanged = true;

            //loading files into first column
            lbContent1.ItemsSource = null;//clear items
            if (!FileSystem.IsRootPath(FileSystem.CurrentPlace.FullName))
            {
                fileSystem1.LoadDirectory(PathExt.GetDirectoryName(FileSystem.CurrentPlace.FullName, FileSystem.IsWindowsFileSystem));
                lbContent1.ItemsSource = fileSystem1.Items;
            }

            //save in history
            if (saveInHistory)
            {
                HistoryGlobal.AddItem(FileSystem.CurrentPlace);
                history.AddItem(FileSystem.CurrentPlace, false);
            }
        }

        public void LoadSelectedDir()
        {
            LoadDir(SelectedItem.FullName, true);
        }

        public void MoveBack()
        {
            LoadDir(history.MoveBack().FullName, false);
        }

        public void MoveForward()
        {
            LoadDir(history.MoveForward().FullName, false);
        }

        public void MoveUp()
        {
            if (!FileSystem.IsRootPath(FileSystem.CurrentPlace.FullName))
                LoadDir(PathExt.GetDirectoryName(DisplayPath, FileSystem.IsWindowsFileSystem), true);
        }

        public void SelectAll()
        {
            lbContent2.SelectAll();
            if (Items.Count > 1 && ((IDirectoryViewItem)Items[0]).IsMoveUp)
                lbContent2.SelectedItems.Remove(Items[0]);
        }

        public void UnselectAll()
        {
            lbContent2.UnselectAll();
        }

        public void SetFocusOnContent()
        {
            //if nothing is selected
            if (SelectedItem == null)
            {
                //focus on 1st item in most cases, on 0th if it is drive root or dir is empty
                lbContent2.SelectedIndex = Items.Count == 1 || PathExt.IsDriveRoot(DisplayPath) ? 0 : 1;
                //lbContent2.ScrollIntoView(SelectedItem); - bug when calling copy on multiple objects not in the view
            }

            ListBoxItem item = lbContent2.ItemContainerGenerator.ContainerFromItem(SelectedItem) as ListBoxItem;
            if (item != null)
                item.Focus();//If item==null AfterCtorInit() call is needed
        }

        /// <summary>
        /// Serve the event of changing selected item in middle ListBox - lbContent2
        /// </summary>
        private void lbContent2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItemsCount > 0)
            {
                lbContent3.ItemsSource = null;//clear items
                IDirectoryViewItem item = (IDirectoryViewItem)SelectedItems[0];
                if (item.IsDirectory)
                {
                    fileSystem3.LoadDirectory(item.FullName);
                    lbContent3.ItemsSource = fileSystem3.Items;
                }
                //TODO: Do preview for files
            }
        }

        private void ThreeColumnView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem clicked = (ListBoxItem)sender;
            IDirectoryViewItem item = (IDirectoryViewItem)clicked.Content;
            OnItemExecuted(item);
        }

        protected void OnItemExecuted(IDirectoryViewItem item)
        {
            var handler = ItemExecuted;
            if (handler != null)
                handler(item, null);
        }

        public void ChangeFileSystem(FileSystemBase fileSystem)
        {
            fileSystem1 = fileSystem.GetCopy();
            fileSystem2 = fileSystem;
            fileSystem3 = fileSystem.GetCopy();

            //set up binding for second column
            Binding bind2 = new Binding("Items");//bind to  ListBox
            bind2.Source = FileSystem;
            lbContent2.SetBinding(ListBox.ItemsSourceProperty, bind2);

            //loading files into first column
            lbContent1.ItemsSource = null;//clear items
            if (!FileSystem.IsRootPath(FileSystem.CurrentPlace.FullName))
            {
                fileSystem1.LoadDirectory(PathExt.GetDirectoryName(FileSystem.CurrentPlace.FullName, FileSystem.IsWindowsFileSystem));
                lbContent1.ItemsSource = fileSystem1.Items;
            }

            //third column is loaded by selecting items in second

            //save in history
            HistoryGlobal.AddItem(FileSystem.CurrentPlace);
            history.AddItem(FileSystem.CurrentPlace, false);

            dirChanged = true;
            //SetFocusOnContent();
            OnPropertyChanged("FileSystem");
        }
    }
}