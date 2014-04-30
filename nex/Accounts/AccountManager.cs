using System.Collections.ObjectModel;
using System.Linq;
using lib12.DependencyInjection;
using lib12.WPF.Serialization;

namespace nex.Accounts
{
    [Singleton]
    public class AccountManager : SerializableViewModel
    {
        #region Props
        [SerializeProperty(0)]
        public int NextId { get; private set; }
        [SerializeProperty(CreateNewAsDefaultValue = true)]
        public ObservableCollection<Account> Accounts { get; set; }
        #endregion

        #region Logic
        public int GetNextId()
        {
            return NextId++;
        }

        public void AddNewAccount(Account account)
        {
            Accounts.Add(account);
        }

        public Account GetAccountById(int id)
        {
            return (from a in Accounts
                    where a.Id == id
                    select a).Single();
        }

        public Account FindCurrentDefault()
        {
            return (from a in Accounts
                    where a.IsDefault
                    select a).FirstOrDefault();
        } 
        #endregion
    }
}