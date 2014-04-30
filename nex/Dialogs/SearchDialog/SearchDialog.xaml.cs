using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using nex.FileSystem;

namespace nex.Dialogs.SearchDialog
{
    /// <summary>
    /// Interaction logic for SearchDialog.xaml
    /// </summary>
    public partial class SearchDialog
    {
        #region Fields
        private Thread thread; 
        #endregion

        #region Props
        public bool IsSearching { get; private set; }
        public string FoundObject { get; private set; }
        #endregion

        #region ctor
        public SearchDialog()
        {
            IsSearching = false;
            FoundObject = null;
            InitializeComponent();
            tToFind.Focus();
        } 
        #endregion

        private void KeyDown_FindDialog(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            else if (e.Key == Key.Enter)
                bSearch_Click(null, null);
        }

        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            if (IsSearching)//Stop clicked
            {
                thread.Abort();
                tToFind.IsEnabled = true;
                bSearch.Content = "Szukaj";
                IsSearching = false;
            }
            else //Start clicked
            {
                if (tToFind.Text == string.Empty)
                {
                    MessageBox.Show("Podaj nazwę pliku/katalogu do znalezienia!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                lbFindItems.Items.Clear();
                IsSearching = true;
                tToFind.IsEnabled = false;
                bSearch.Content = "Zatrzymaj";
                thread = new Thread(new ParameterizedThreadStart(Search));
                thread.Start(tToFind.Text);
            }
        }

        private void Search(object objToFind)
        {
            string toFind = (string)objToFind;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                    break;

                if (!drive.IsReady)
                    continue;

                SearchDirectory(drive.RootDirectory, toFind);
            }
        }

        private void SearchDirectory(DirectoryInfo directory, string toFind)
        {
            if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                return;

            NewDirVisited(directory.Name);
            try
            {
                foreach (FileInfo file in directory.GetFiles(toFind))
                    if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        ObjectFind(file.FullName);

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    //skip hidden files
                    if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                    if (dir.Name == toFind)
                        ObjectFind(dir.FullName);

                    SearchDirectory(dir, toFind);
                }
            }
            catch
            {
                //access to file/dir is denied
            }
        }

        private void NewDirVisited(string dir)
        {
            if (Dispatcher.CheckAccess())
                tStatus.Text = dir;
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NewDirVisited), dir);
        }

        private void ObjectFind(string path)
        {
            if (Dispatcher.CheckAccess())
                lbFindItems.Items.Add(path);
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(ObjectFind), path);
        }

        private void lbFindItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string path = lbFindItems.SelectedItem.ToString();
            FoundObject = PathExt.IsPathToDirectory(path) ? path : Path.GetDirectoryName(path);
            thread.Abort();
            DialogResult = true;
        }
    }
}