using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using lib12.Collections;
using lib12.DependencyInjection;
using lib12.Extensions;
using lib12.WPF.Core;
using nex.FileSystem;
using nex.FileSystem.Windows;
using nex.HistoryLogic;
using nex.PathManager;
using nex.Utilities;

namespace nex.DirectoryView
{
    [Transient]
    public sealed class DirectoryViewModel : NotifyingObject
    {
        #region Fields
        private PropertyObserver<DirectoryViewModel> propObserver;
        private FileSystemWatcher fileSystemWatcher;
        #endregion

        #region Props
        private IDirectoryViewItem[] items;
        public IDirectoryViewItem[] Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        private string fullPath;
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value;
                OnPropertyChanged("FullPath");
            }
        }

        private string directoryName;
        public string DirectoryName
        {
            get
            {
                return directoryName;
            }
            set
            {
                directoryName = value;
                OnPropertyChanged("DirectoryName");
            }
        }

        private bool isSearchEnabled;
        public bool IsSearchEnabled
        {
            get
            {
                return isSearchEnabled;
            }
            set
            {
                isSearchEnabled = value;
                OnPropertyChanged("IsSearchEnabled");
            }
        }

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private bool isFavoritePath;
        public bool IsFavoritePath
        {
            get
            {
                return isFavoritePath;
            }
            set
            {
                isFavoritePath = value;
                OnPropertyChanged("IsFavoritePath");
            }
        }

        private IDirectoryViewItem selectedItem;
        public IDirectoryViewItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private DirectoryViewType viewType;
        public DirectoryViewType ViewType
        {
            get { return viewType; }
            set
            {
                viewType = value;
                OnPropertyChanged("ViewType");
            }
        }

        public bool IsMoveUpSelected
        {
            get
            {
                return SelectedItem != null && SelectedItem.IsMoveUp && SelectedItems.ContainsOneElement();
            }
        }

        public bool IsOneFileSelected
        {
            get
            {
                return SelectedItems.ContainsOneElement() && !IsMoveUpSelected && !SelectedItem.IsDirectory;
            }
        }

        public bool IsAtLeastOneItemSelected
        {
            get
            {
                return SelectedItem != null && !SelectedItem.IsMoveUp;
            }
        }

        public int SelectedItemsCount
        {
            get
            {
                return SelectedItems.Length;
            }
        }

        [WireUp]
        public PathsManager PathsManager { get; set; }
        public FileSystemBase FileSystem { get; private set; }
        public object[] SelectedItems { get; set; }
        public History<IDirectoryViewItem> History { get; private set; }
        public ICommand ExecuteItemCommand { get; private set; }
        public bool IsAddTab { get; private set; }
        #endregion

        #region ctor && dctor
        public DirectoryViewModel()
        {
            ExecuteItemCommand = new DelegateCommand(ExecuteExecuteItem, CanExecuteExecuteItem);
            History = new History<IDirectoryViewItem>();
            FileSystem = new WindowsFileSystem();

            propObserver = new PropertyObserver<DirectoryViewModel>(this);
            propObserver.RegisterHandler(x => x.SearchText, ManageSearch);

            fileSystemWatcher = new FileSystemWatcher { IncludeSubdirectories = false, };
            fileSystemWatcher.Created += fileSystemWatcher_Changed;
            fileSystemWatcher.Renamed += fileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += fileSystemWatcher_Changed;
        }

        public static DirectoryViewModel CreateAddTabViewModel()
        {
            return new DirectoryViewModel { IsAddTab = true, DirectoryName = "+" };
        }

        ~DirectoryViewModel()
        {
            fileSystemWatcher.Changed -= fileSystemWatcher_Changed;
            fileSystemWatcher.Dispose();
        }

        #endregion

        #region Commands
        private void ExecuteExecuteItem(object parameter)
        {
            if (SelectedItem.IsDirectory)
                LoadDirectory(SelectedItem.FullName);
            else if (SelectedItem.IsMoveUp)
                MoveUp();
            else
                ExecuteFile(SelectedItem);
        }

        private bool CanExecuteExecuteItem(object parameter)
        {
            return SelectedItem != null;
        }

        #endregion

        #region Search
        private void ManageSearch()
        {
            if (SearchText == string.Empty)
            {
                IsSearchEnabled = false;
                foreach (var item in Items)//for some weird reason ForEach function don't work
                    item.IsMatchingCriteria = null;
            }
            else
            {
                IsSearchEnabled = true;
                Items = Items.ForEach(x => x.IsMatchingCriteria = x.Name.ToUpper().Contains(SearchText.ToUpper()))
                             .OrderByDescending(x => (x.IsMoveUp || x.IsMatchingCriteria.IsTrue()))
                             .ToArray();
            }
        }

        private void DisableSearch()
        {
            SearchText = string.Empty;
        }

        #endregion

        #region FileSystem logic
        public void LoadDirectory(string dir, bool moveUp = false, bool saveInHistory = true)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            var lastPlace = FileSystem.CurrentPlace;
            FileSystem.LoadDirectory(dir);
            Items = FileSystem.Items.OrderByDescending(x => x.IsMoveUp || x.IsDirectory).ToArray();
            DirectoryName = FileSystem.DirectoryName;
            FullPath = FileSystem.FullPath;
            IsFavoritePath = PathsManager.Favorites.Contains(FileSystem.CurrentPlace);
            SelectedItem = moveUp ? Items.First(x => x.FullName == lastPlace.FullName) : GetItemToSelect();

            if (FileSystem.IsWindowsFileSystem)
            {
                fileSystemWatcher.Path = dir;
                fileSystemWatcher.EnableRaisingEvents = true;
            }

            if (saveInHistory)
            {
                HistoryGlobal.AddItem(FileSystem.CurrentPlace);
                History.AddItem(FileSystem.CurrentPlace, false);
            }
        }

        private IDirectoryViewItem GetItemToSelect()
        {
            if (Items.ContainsMultipleElements())
                return Items.First(x => !x.IsMoveUp);
            else
                return Items.First();
        }

        public void LoadSelectedDirectory()
        {
            DisableSearch();
            LoadDirectory(SelectedItem.FullName);
        }

        public void ExecuteFile(IDirectoryViewItem item)
        {
            var proccessStartInfo = new ProcessStartInfo(item.FullName);
            Process.Start(proccessStartInfo);
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LoadDirectory(FullPath, false, false);
        }

        public void Refresh()
        {
            if (!FileSystem.IsWindowsFileSystem)
                LoadDirectory(FullPath, false, false);
        }

        #endregion

        #region Move
        public void MoveUp()
        {
            if (!FileSystem.IsRootPath(FileSystem.CurrentPlace.FullName))
                LoadDirectory(PathExt.GetDirectoryName(FullPath, FileSystem.IsWindowsFileSystem), true);
        }

        public void MoveBack()
        {
            if (History.IsNotEmpty())
                LoadDirectory(History.MoveBack().FullName, false, false);
        }

        public void MoveForward()
        {
            if (History.IsNotEmpty())
                LoadDirectory(History.MoveForward().FullName, false, false);
        }

        #endregion

        public void ChangeFileSystem(FileSystemBase fileSystem)
        {
            DisableSearch();
            FileSystem.Dispose();
            FileSystem = fileSystem;
            LoadDirectory(FileSystem.CurrentPlace.FullName);
        }

        public void SelectAll()
        {
        }

        public void UnselectAll()
        {
        }

        public void SetFocusOnContent()
        {
        }
    }
}
