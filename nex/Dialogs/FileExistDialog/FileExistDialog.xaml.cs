using System.Text;
using System.Windows;
using nex.Controls.Dialogs.OperationErrors;
using nex.DirectoryView;

namespace nex.Dialogs.FileExistDialog
{
    /// <summary>
    /// Interaction logic for FileExistDialog.xaml
    /// </summary>
    public partial class FileExistDialog
    {
        public const FileExistDalogResult DefaultResult = FileExistDalogResult.DontOverride;

        public FileExistDalogResult Result { get; private set; }

        public FileExistDialog()
        {
            InitializeComponent();
            Result = DefaultResult;
        }

        public FileExistDialog(IDirectoryViewItem existingFile, string destination)
        {
            InitializeComponent();
            Result = DefaultResult;

            tInfo.Text = CreateInfoMessage(existingFile, destination);
        }

        private string CreateInfoMessage(IDirectoryViewItem existingFile, string destination)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(existingFile.IsDirectory ? "Katalog " : "Plik ");
            sb.Append("o nazwie ");
            sb.Append(existingFile.Name);
            sb.Append(" już istnieje w katalogu ");
            sb.Append(destination);
            sb.Append("! Co chcesz teraz zrobić?");

            return sb.ToString();
        }

        #region Events Support

        private void bOverride_Click(object sender, RoutedEventArgs e)
        {
            Result = FileExistDalogResult.Override;
            DialogResult = true;
        }

        private void bOverrideAll_Click(object sender, RoutedEventArgs e)
        {
            Result = FileExistDalogResult.OverrideAll;
            DialogResult = true;
        }

        private void bDontOverride_Click(object sender, RoutedEventArgs e)
        {
            Result = FileExistDalogResult.DontOverride;
            DialogResult = true;
        }

        private void bCancelOperation_Click(object sender, RoutedEventArgs e)
        {
            Result = FileExistDalogResult.CancelOperation;
            DialogResult = true;
        }
        #endregion
    }
}