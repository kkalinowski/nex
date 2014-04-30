using System.Windows;
using nex.Accounts;

namespace nex.Dialogs.AccountDialogLogic
{
    /// <summary>
    /// Interaction logic for AccountDialog.xaml
    /// </summary>
    public partial class AccountDialog
    {
        #region Props
        public Account Account { get; private set; }
        #endregion

        public AccountDialog()
        {
            InitializeComponent();
        }

        public void EditAccount(Account acc)
        {
            //OPT: Need to edit a type 
            tUrl.Text = acc.Url;
            tUserName.Text = acc.UserName;
            tPassword.Password = acc.Password;
            chbIsDefault.IsChecked = acc.IsDefault;
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if (tUrl.Text == string.Empty)
            {
                MessageBox.Show("Podaj adres serwisu");
                return;
            }

            if (tUserName.Text == string.Empty)
            {
                MessageBox.Show("Podaj nazwę użytkownika");
                return;
            }

            Account = new Account(tUrl.Text, tUserName.Text, tPassword.Password, chbIsDefault.IsChecked.Value);
            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}