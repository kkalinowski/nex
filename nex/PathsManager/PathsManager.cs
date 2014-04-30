using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using lib12.DependencyInjection;
using lib12.WPF.Serialization;
using nex.DirectoryView;
using nex.FileSystem.Windows;
using nex.HistoryLogic;

namespace nex.PathManager
{
    [Singleton]
    public class PathsManager : SerializableViewModel
    {
        #region Props
        public ObservableCollection<WindowsFile> SystemPaths { get; private set; }
        [SerializeProperty(CreateNewAsDefaultValue = true)]
        public ObservableCollection<IDirectoryViewItem> Favorites { get; private set; }
        public IEnumerable<IDirectoryViewItem> History { get; private set; }
        #endregion

        #region Start
        public PathsManager()
        {
            CreateSystemPathsList();
            History = HistoryGlobal.BaseHistory.Reversed;
        }

        private void CreateSystemPathsList()
        {
            SystemPaths = new ObservableCollection<WindowsFile>();

            //add drives
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    SystemPaths.Add(new WindowsFile(new DirectoryInfo(drive.Name)));
            }

            //add special folders
            foreach (Environment.SpecialFolder folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string path = Environment.GetFolderPath(folder);
                if (!string.IsNullOrEmpty(path))
                    SystemPaths.Add(new WindowsFile(new DirectoryInfo(path)));
            }
        }
        #endregion

        #region Favorites
        public bool IsFavorite(IDirectoryViewItem toCheck)
        {
            return Favorites.Contains(toCheck);
        }

        public void AddFavorite(IDirectoryViewItem toAdd)
        {
            Favorites.Add(toAdd);
        }

        public void RemoveFavorite(IDirectoryViewItem toRemove)
        {
            Favorites.Remove(toRemove);
        }
        #endregion
    }
}
