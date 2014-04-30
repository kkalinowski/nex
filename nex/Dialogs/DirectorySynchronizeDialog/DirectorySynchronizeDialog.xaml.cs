using System.Linq;
using nex.DirectoryView;
using ElysiumWindow = Elysium.Controls.Window;
using System.Windows;
using System.Windows.Forms;
using nex.FileSystem.Windows;
using System.Collections.Generic;

namespace nex.Dialogs.DirectorySynchronizeDialog
{
    /// <summary>
    /// Interaction logic for DirectorySynchronizeDialog.xaml
    /// </summary>
    public partial class DirectorySynchronizeDialog : ElysiumWindow
    {
        #region Fields
        private List<DirectoryComparison> comparisonResult;
        #endregion

        #region PropDP
        public string LeftDir
        {
            get
            {
                return (string)GetValue(LeftDirProperty);
            }
            set
            {
                SetValue(LeftDirProperty, value);
            }
        }

        public static readonly DependencyProperty LeftDirProperty =
            DependencyProperty.Register("LeftDir", typeof(string), typeof(DirectorySynchronizeDialog));

        public string RightDir
        {
            get
            {
                return (string)GetValue(RightDirProperty);
            }
            set
            {
                SetValue(RightDirProperty, value);
            }
        }

        public static readonly DependencyProperty RightDirProperty =
            DependencyProperty.Register("RightDir", typeof(string), typeof(DirectorySynchronizeDialog));
        #endregion

        #region Props

        public DirectorySynchronizeResult SyncResult { get; private set; }
        #endregion

        public DirectorySynchronizeDialog(string leftDir, string rightDir)
        {
            LeftDir = leftDir;
            RightDir = rightDir;

            InitializeComponent();
        }

        private void bChangeDir_Click(object sender, RoutedEventArgs e)
        {
            bool left = sender == bChangeLeftDir;
            FolderBrowserDialog dirDialog = new FolderBrowserDialog();
            dirDialog.SelectedPath = left ? tLeftDir.Text : tRightDir.Text;

            if (dirDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (left)
                    tLeftDir.Text = dirDialog.SelectedPath;
                else
                    tRightDir.Text = dirDialog.SelectedPath;
            }
        }

        private void bCompare_Click(object sender, RoutedEventArgs e)
        {
            //load directory content
            WindowsFileSystem fileSystem = new WindowsFileSystem();
            IEnumerable<IDirectoryViewItem> leftItems = fileSystem.GetDirectoryContent(LeftDir);
            List<IDirectoryViewItem> rightItems = fileSystem.GetDirectoryContent(RightDir).ToList();//list need for remove functionality

            comparisonResult = new List<DirectoryComparison>();
            IDirectoryViewItem rightItem;

            foreach (IDirectoryViewItem item in leftItems)
            {
                if (item == null || item.IsMoveUp)
                    continue;

                rightItem = rightItems.SingleOrDefault(dvi => dvi != null && dvi.Name == item.Name);//right item that matches left name
                if (rightItem != null)
                {
                    if (item.IsDirectory)
                        comparisonResult.Add(new DirectoryComparison(item, rightItem, DirectoryComparisonResult.Equal));
                    else if (item.LastModifiedTime > rightItem.LastModifiedTime)
                        comparisonResult.Add(new DirectoryComparison(item, rightItem, DirectoryComparisonResult.LeftNewer));
                    else if (item.LastModifiedTime < rightItem.LastModifiedTime)
                        comparisonResult.Add(new DirectoryComparison(item, rightItem, DirectoryComparisonResult.RightNewer));
                    else
                        comparisonResult.Add(new DirectoryComparison(item, rightItem, DirectoryComparisonResult.Equal));

                    rightItems.Remove(rightItem);//no need in futher comparison
                }
                else
                {
                    comparisonResult.Add(new DirectoryComparison(item, null, DirectoryComparisonResult.LeftNewer));
                }
            }

            //every item from right collection don't exist in left
            foreach (IDirectoryViewItem item in rightItems)
            {
                if (item == null || item.IsMoveUp)
                    continue;

                comparisonResult.Add(new DirectoryComparison(null, item, DirectoryComparisonResult.RightNewer));
            }

            lvCompare.ItemsSource = comparisonResult;
        }

        private void bSynchronize_Click(object sender, RoutedEventArgs e)
        {
            if (comparisonResult == null)
                return;

            SyncResult = new DirectorySynchronizeResult(new List<DirectoryComparison>(), LeftDir, RightDir);

            foreach (DirectoryComparison comp in comparisonResult)
                if (comp.Synchronize)
                    SyncResult.Comparison.Add(comp);

            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
