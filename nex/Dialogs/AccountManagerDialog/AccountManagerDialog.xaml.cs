using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using lib12.DependencyInjection;
using nex.Accounts;
using nex.Dialogs.AccountDialogLogic;
using nex.Utilities;

namespace nex.Dialogs.AccountManagerDialog
{
    /// <summary>
    /// Interaction logic for AccountManagerDialog.xaml
    /// </summary>
    [Transient]
    public partial class AccountManagerDialog
    {
        #region PropDP
        public bool IsCancelable
        {
            get
            {
                return (bool)GetValue(IsCancelableProperty);
            }
            set
            {
                SetValue(IsCancelableProperty, value);
            }
        }

        public static readonly DependencyProperty IsCancelableProperty =
            DependencyProperty.Register("IsCancelable", typeof(bool), typeof(AccountManagerDialog));
        #endregion

        #region Props
        public Account SelectedAccount
        {
            get
            {
                return (Account)lbAccounts.SelectedItem;
            }
        }
        public AccountManager AccountManager { get; private set; }
        #endregion

        public AccountManagerDialog(AccountManager accountManager)
        {
            InitializeComponent();

            AccountManager = accountManager;
            var accBind = new Binding("Accounts");
            accBind.Source = AccountManager;
            lbAccounts.SetBinding(ListBox.ItemsSourceProperty, accBind);

            if (lbAccounts.Items.Count > 0)
                lbAccounts.SelectedIndex = 0;
        }

        private void bNew_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AccountDialog();
            if (dialog.ShowModalDialog())
            {
                AccountManager.AddNewAccount(dialog.Account);
                lbAccounts.SelectedIndex = lbAccounts.Items.Count - 1;
            }
        }

        private void bEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbAccounts.SelectedItem != null)
            {
                Account acc = (Account)lbAccounts.SelectedItem;
                AccountDialog dialog = new AccountDialog();
                dialog.EditAccount(acc);

                if (dialog.ShowModalDialog())
                {
                    //QSTN: Can this copy be done better?
                    acc.Url = dialog.Account.Url;
                    acc.UserName = dialog.Account.UserName;
                    acc.Password = dialog.Account.Password;

                    if (dialog.Account.IsDefault)
                    {
                        Account oldDefault = AccountManager.FindCurrentDefault();
                        if (oldDefault != null)
                            oldDefault.IsDefault = false;

                        acc.IsDefault = true;
                    }
                }
            }
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbAccounts.SelectedItem != null)
            {
                Account acc = (Account)lbAccounts.SelectedItem;
                AccountManager.Accounts.Remove(acc);
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsCancelable && SelectedAccount == null)
                return;

            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void bSetDefault_Click(object sender, RoutedEventArgs e)
        {
            //OPT: Update changes in gui
            if (lbAccounts.SelectedItem != null)
            {
                Account oldDefault = AccountManager.FindCurrentDefault();//set old Default as no default
                if (oldDefault != null)
                    oldDefault.IsDefault = false;

                ((Account)lbAccounts.SelectedItem).IsDefault = true;
            }
        }
    }
}