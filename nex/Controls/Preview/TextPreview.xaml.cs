using System.Windows.Controls;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for TextPreview.xaml
    /// </summary>
    public partial class TextPreview : UserControl
    {
        public string Text
        {
            get
            {
                return tContent.Text;
            }
            set
            {
                tContent.Text = value;
            }
        }

        public TextPreview()
        {
            InitializeComponent();
        }
    }
}