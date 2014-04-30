using System.Windows;

namespace nex.Dialogs.CompressDialog
{
    /// <summary>
    /// Interaction logic for CompressDialog.xaml
    /// </summary>
    public partial class CompressDialog
    {
        #region Props
        public string FileName { get; private set; }
        public string Password { get; private set; }
        #endregion

        public CompressDialog()
        {
            InitializeComponent();
            tName.Focus();
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if (tName.Text == string.Empty)
            {
                MessageBox.Show("Musisz podać nazwę pliku!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FileName = tName.Text.EndsWith(".zip") ? tName.Text : tName.Text + ".zip";
            Password = tPassword.Text;
            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}