using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class ImagePreview : UserControl
    {
        public ImagePreview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load image file
        /// </summary>
        /// <param name="path">Path to image file</param>
        public void LoadImage(string path)
        {
            BitmapImage imgSource = new BitmapImage(new Uri(path));
            //OPT: Use img.DecodePixelWidth to speed up program
            imgContent.Source = imgSource;
        }
    }
}