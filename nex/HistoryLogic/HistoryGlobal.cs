using System.Collections.ObjectModel;
using nex.DirectoryView;
using nex.Utilities.Serialization;

namespace nex.HistoryLogic
{
    public static class HistoryGlobal
    {
        private const string SerializationKey = "History_Global";

        #region Fields
        private static History<IDirectoryViewItem> history;
        #endregion

        #region Props
        public static ObservableCollection<IDirectoryViewItem> Items
        {
            get
            {
                return history.Items;
            }
            internal set
            {
                history.Items = value;
            }
        }

        public static int Capacity
        {
            get
            {
                return history.Capacity;
            }
            set
            {
                history.Capacity = value;
            }
        }

        /// <summary>
        /// Get or set currenct position in history
        /// </summary>
        public static int Position
        {
            get
            {
                return history.Position;
            }
            set
            {
                history.Position = value;
            }
        }

        /// <summary>
        /// Get underlying History object
        /// </summary>
        public static History<IDirectoryViewItem> BaseHistory
        {
            get
            {
                return history;
            }
        }
        #endregion

        static HistoryGlobal()
        {
            history = new History<IDirectoryViewItem>();
            history.SerializationKey = SerializationKey;

            SerializationHelper.RegisterUser(history);
            if (SerializationHelper.DataLoaded)
            {
                history.ApplyLoadedData();
            }
        }

        public static void AddItem(IDirectoryViewItem item)
        {
            history.AddItem(item, true);
        }
    }
}