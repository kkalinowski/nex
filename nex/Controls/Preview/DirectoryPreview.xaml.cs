using System.Collections.Generic;
using System.Windows.Controls;
using nex.DirectoryView;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for DirectoryPreview.xaml
    /// </summary>
    public partial class DirectoryPreview : UserControl
    {
        public DirectoryPreview()
        {
            InitializeComponent();
        }

        public void LoadDirectory(IEnumerable<IDirectoryViewItem> items)
        {
            lbContent.ItemsSource = items;
        }
    }
}