using System.IO;
using System.Windows;

namespace nex.Dialogs.RenameDialog
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog
    {
        #region Props
        public string ToRename { get; set; }

        public string NewName
        {
            get
            {
                return tNewName.Text;
            }
        }
        #endregion

        #region ctor
        public RenameDialog()
        {
            InitializeComponent();
        }

        #endregion

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tPath.Text = ToRename;
            tOldName.Text = Path.GetFileName(ToRename);
            tNewName.Text = Path.GetFileName(ToRename);

            tNewName.Focus();
            tNewName.Select(0, Path.GetFileNameWithoutExtension(ToRename).Length);
        }
    }
}