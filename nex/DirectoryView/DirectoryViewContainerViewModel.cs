using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using lib12.Collections;
using lib12.DependencyInjection;
using lib12.IO;
using lib12.Serialization;
using lib12.WPF.Core;
using lib12.WPF.EventTranscriptions;
using nex.FileSystem;
using nex.FileSystem.Windows;
using nex.Operations;
using nex.PathManager;
using nex.Utilities;

namespace nex.DirectoryView
{
    [Transient]
    public sealed class DirectoryViewContainerViewModel : NotifyingObject
    {
        #region Fields
        private bool addingView;
        private IDirectoryViewItem[] droppedFiles;
        #endregion

        #region Props
        private DirectoryViewModel activeView;
        public DirectoryViewModel ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                if (value.IsAddTab)
                {
                    AddNewDirectoryView();
                }
                else
                {
                    if (activeView != null)
                        activeView.SearchText = string.Empty;
                    activeView = value;
                    OnPropertyChanged("ActiveView");
                }
            }
        }

        public IDirectoryViewItem SelectedPath
        {
            get
            {
                return null;
            }
            set
            {
                LoadPath(value);
                OnPropertyChanged("SelectedPath");
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        private bool isDropMenuOpen;
        public bool IsDropMenuOpen
        {
            get { return isDropMenuOpen; }
            set
            {
                isDropMenuOpen = value;
                OnPropertyChanged("IsDropMenuOpen");
            }
        }

        [WireUp]
        public PathsManager PathsManager { get; set; }
        [WireUp]
        public OperationsManager OperationsManager { get; set; }
        public string SerializationKey { get; set; }
        public ObservableCollection<DirectoryViewModel> Directories { get; private set; }
        public ICommand CloseTabCommand { get; private set; }
        public ICommand KeyboardCommand { get; private set; }
        public ICommand RightMouseCommand { get; private set; }
        public ICommand HandleFavoritePathCommand { get; private set; }
        public ICommand DropCommand { get; private set; }
        public ICommand DropCopyCommand { get; private set; }
        public ICommand DropMoveCommand { get; private set; }
        #endregion

        #region ctor
        public DirectoryViewContainerViewModel()
        {
            SerializationHelper.Current.SerializationStarted += SerializationStarted;

            CloseTabCommand = new DelegateCommand<DirectoryViewModel>(ExecuteCloseTab, CanExecuteCloseTab);
            KeyboardCommand = new DelegateCommand<EventTranscriptionParameter<KeyEventArgs>>(ExecuteKeyboard);
            RightMouseCommand = new DelegateCommand<EventTranscriptionParameter<MouseButtonEventArgs>>(ExecuteRightMouse);
            HandleFavoritePathCommand = new DelegateCommand<EventTranscriptionParameter<MouseButtonEventArgs>>(ExecuteHandleFavoritePath);
            DropCommand = new DelegateCommand<EventTranscriptionParameter<DragEventArgs>>(ExecuteDrop);
            DropCopyCommand = new DelegateCommand(ExecuteDropCopy);
            DropMoveCommand = new DelegateCommand(ExecuteDropMove);
        }

        public void LoadDirectories()
        {
            Directories = new ObservableCollection<DirectoryViewModel>();
            var dict = SerializationHelper.Current.Data.GetValueOrDefault(SerializationKey) as Dictionary<string, object>;
            if (dict != null)
            {
                var paths = (string[])dict["Paths"];
                var views = (DirectoryViewType[])dict["Views"];
                System.Diagnostics.Debug.Assert(paths.Length == views.Length);
                for (int i = 0; i < paths.Length; i++)
                {
                    var view = Instances.Get<DirectoryViewModel>();
                    view.ViewType = views[i];
                    if (Directory.Exists(paths[i]))
                        view.LoadDirectory(paths[i]);
                    else
                        view.LoadDirectory(IOHelper.GetDefaultPath());
                    Directories.Add(view);
                }
            }
            else
            {
                var view = Instances.Get<DirectoryViewModel>();
                view.LoadDirectory(IOHelper.GetDefaultPath());
                Directories.Add(view);
            }

            Directories.Add(DirectoryViewModel.CreateAddTabViewModel());
        }
        #endregion

        #region Serialization
        private void SerializationStarted(object sender, SerializationStartedEventArgs e)
        {
            Directories.Where(x => !x.FileSystem.IsWindowsFileSystem).ForEach(x => x.ChangeFileSystem(new WindowsFileSystem(IOHelper.GetDefaultPath())));

            var dict = new Dictionary<string, object>(2);
            dict.Add("Paths", Directories.Where(x => !x.IsAddTab).Select(x => x.FullPath).ToArray());
            dict.Add("Views", Directories.Where(x => !x.IsAddTab).Select(x => x.ViewType).ToArray());
            e.Data.Add(SerializationKey, dict);
        }
        #endregion

        #region Tab managment
        private void AddNewDirectoryView()
        {
            if (addingView)
                return;

            addingView = true;
            var view = Instances.Get<DirectoryViewModel>();
            view.LoadDirectory(ActiveView.FullPath);
            Directories.Insert(Directories.Count - 1, view);
            ActiveView = view;
            addingView = false;
        }

        private void ExecuteCloseTab(DirectoryViewModel parameter)
        {
            if (parameter == ActiveView)
            {
                var closingIndex = Directories.IndexOf(parameter);
                var newActiveView = closingIndex == 0 ? Directories[1] : Directories[closingIndex - 1];
                ActiveView = newActiveView;
            }
            Directories.Remove(parameter);
        }

        private bool CanExecuteCloseTab(DirectoryViewModel parameter)
        {
            return Directories.Count > 2;
        }

        #endregion

        #region Keyboard and Mouse
        private void ExecuteKeyboard(EventTranscriptionParameter<KeyEventArgs> parameter)
        {
            var args = parameter.EventArgs;
            parameter.EventArgs.Handled = true;

            if (parameter.EventArgs.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && parameter.EventArgs.KeyboardDevice.IsKeyDown(Key.T))
                AddNewDirectoryView();
            else if (parameter.EventArgs.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && parameter.EventArgs.KeyboardDevice.IsKeyDown(Key.W))
                ExecuteCloseTab(ActiveView);
            else if (args.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                args.Handled = false;
            else if (parameter.EventArgs.Key == Key.Enter && ActiveView.SelectedItem != null && ActiveView.SelectedItem.IsDirectory)
                ActiveView.LoadSelectedDirectory();
            else if (parameter.EventArgs.Key == Key.Enter && ActiveView.SelectedItem != null && ActiveView.SelectedItem.IsMoveUp && !PathExt.IsDriveRoot(ActiveView.FullPath))
                ActiveView.MoveUp();
            else if (parameter.EventArgs.Key == Key.Back && ActiveView.SearchText.IsNotNullAndNotEmpty())
                ActiveView.SearchText = ActiveView.SearchText.Remove(ActiveView.SearchText.Length - 1);
            else if (parameter.EventArgs.Key == Key.Back && !PathExt.IsDriveRoot(ActiveView.FullPath))
                ActiveView.MoveUp();
            else if (parameter.EventArgs.Key == Key.Enter && ActiveView.IsOneFileSelected)
                ActiveView.ExecuteFile(ActiveView.SelectedItem);
            else
            {
                char keyID = Utility.ConvertKeyToChar(parameter.EventArgs.Key);
                if (keyID != char.MinValue)
                {
                    if (char.IsLetterOrDigit(keyID))
                        ActiveView.SearchText = ActiveView.SearchText + keyID.ToString();
                    else if (parameter.EventArgs.Key == Key.Space)//TODO: Handle space
                        ActiveView.SearchText = ActiveView.SearchText + " ";
                }
                else
                    parameter.EventArgs.Handled = false;
            }
        }

        private void ExecuteRightMouse(EventTranscriptionParameter<MouseButtonEventArgs> parameter)
        {
            if (ActiveView.FileSystem.IsWindowsFileSystem && ActiveView.IsAtLeastOneItemSelected)
            {
                var menu = new WindowsContextMenu();
                var files = ActiveView.SelectedItems.OfType<WindowsFile>().Select(x => (FileSystemInfo)x).ToArray();
                var mousePoint = Mouse.PrimaryDevice.GetPosition(Application.Current.MainWindow);
                mousePoint.Offset(Application.Current.MainWindow.Left, Application.Current.MainWindow.Top);
                menu.ShowContextMenu(files, Utility.ConvertPoint(mousePoint), ActiveView.FullPath);
            }

            parameter.EventArgs.Handled = true;
        }
        #endregion

        #region Path manager
        private void LoadPath(IDirectoryViewItem toLoad)
        {
            if (toLoad == null)
                return;

            if (!ActiveView.FileSystem.IsWindowsFileSystem)
                ActiveView.ChangeFileSystem(new WindowsFileSystem(toLoad.FullName));
            else
                ActiveView.LoadDirectory(toLoad.FullName);
        }

        #endregion

        #region Favorite
        private void ExecuteHandleFavoritePath(EventTranscriptionParameter<MouseButtonEventArgs> parameter)
        {
            if (ActiveView.IsFavoritePath)
            {
                ActiveView.IsFavoritePath = false;
                PathsManager.RemoveFavorite(ActiveView.FileSystem.CurrentPlace);
            }
            else
            {
                ActiveView.IsFavoritePath = true;
                PathsManager.AddFavorite(ActiveView.FileSystem.CurrentPlace);
            }
        }
        #endregion

        #region Drop commands
        private void ExecuteDrop(EventTranscriptionParameter<DragEventArgs> parameter)
        {
            if (!parameter.EventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var filePaths = (string[])parameter.EventArgs.Data.GetData(DataFormats.FileDrop, true);
            droppedFiles = filePaths.Select(x => WindowsFile.CreateFromPath(x)).ToArray();
            IsDropMenuOpen = true;
        }

        private void ExecuteDropCopy(object parameter)
        {
            var sourceFileSystem = new WindowsFileSystem(droppedFiles.First().FullName);
            var operation = new CopyOperation(droppedFiles, ActiveView.FullPath, sourceFileSystem, ActiveView.FileSystem);
            OperationsManager.ExecuteOperation(operation);
        }

        private void ExecuteDropMove(object parameter)
        {
            var sourceFileSystem = new WindowsFileSystem(droppedFiles.First().FullName);
            var operation = new MoveOperation(droppedFiles, ActiveView.FullPath, sourceFileSystem, ActiveView.FileSystem);
            OperationsManager.ExecuteOperation(operation);
        }
        #endregion
    }
}
