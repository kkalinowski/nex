using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using nex.Dialogs.MultiRenameDialog;
using nex.DirectoryView;

namespace nex.Dialogs.MultiRenameDialog
{
    /// <summary>
    /// Interaction logic for MultiRenameDialog.xaml
    /// </summary>
    public partial class MultiRenameDialog
    {
        public ObservableCollection<MultiRenameItem> Items { get; set; }

        public MultiRenameDialog(IList items)
        {
            Items = new ObservableCollection<MultiRenameItem>();
            CreateItems(items);

            InitializeComponent();
            tPattern.Focus();
        }

        private void CreateItems(IList items)
        {
            foreach (IDirectoryViewItem item in items)
                Items.Add(new MultiRenameItem(item));
        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void tPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            //recompute new names
            if (tPattern.Text != string.Empty && PatternContainsMarker())
            {
                StringBuilder newName;
                int counter = 0;
                foreach (MultiRenameItem item in Items)
                {
                    newName = new StringBuilder(tPattern.Text);
                    newName.Replace("[N]", item.Item.NameWithoutExt);
                    newName.Replace("[C]", (counter++).ToString());
                    newName.Append(".");
                    newName.Append(item.Item.Ext);
                    item.NewName = newName.ToString();
                }
            }
        }

        /// <summary>
        /// Check if tPattern text box contains marker to replace - if not, every object will have the same name
        /// </summary>
        /// <returns>True if tPattern contains marker</returns>
        private bool PatternContainsMarker()
        {
            string p = tPattern.Text;//pattern
            return p.Contains("[N]") || p.Contains("[C]");
        }

        private void bNamePattern_Click(object sender, RoutedEventArgs e)
        {
            int caretIndex = tPattern.CaretIndex;
            tPattern.Text = tPattern.Text.Insert(tPattern.CaretIndex, "[N]");
            tPattern.CaretIndex = caretIndex + 3;
            tPattern.Focus();
        }

        private void bCounterPattern_Click(object sender, RoutedEventArgs e)
        {
            int caretIndex = tPattern.CaretIndex;
            tPattern.Text = tPattern.Text.Insert(tPattern.CaretIndex, "[C]");
            tPattern.CaretIndex = caretIndex + 3;
            tPattern.Focus();
        }
    }
}