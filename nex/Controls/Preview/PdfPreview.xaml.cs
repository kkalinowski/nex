using System;
using System.Windows.Controls;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for PdfPreview.xaml
    /// </summary>
    public partial class PdfPreview : UserControl
    {
        public PdfPreview()
        {
            InitializeComponent();
        }

        public void LoadPdf(string path)
        {
            wbContent.Navigate("file:///" + path);
        }
    }
}