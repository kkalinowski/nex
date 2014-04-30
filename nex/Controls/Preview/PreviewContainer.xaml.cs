using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using nex.DirectoryView;
using nex.FileSystem;
using System.Windows.Media;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for PreviewContainer.xaml
    /// </summary>
    public partial class PreviewContainer : UserControl
    {
        #region PropDP - Item
        public IDirectoryViewItem Item
        {
            get
            {
                return (IDirectoryViewItem)GetValue(ItemProperty);
            }
            set
            {
                SetValue(ItemProperty, value);
            }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(IDirectoryViewItem), typeof(PreviewContainer), new PropertyMetadata(new PropertyChangedCallback(ItemChanged)));

        private static void ItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PreviewContainer pv = sender as PreviewContainer;
            pv.LoadItem((IDirectoryViewItem)e.NewValue);
        }

        #endregion

        public PreviewContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load file to preview
        /// </summary>
        /// <param name="file">File to preview</param>
        private void LoadItem(IDirectoryViewItem file)
        {
            if (file.IsMoveUp)
                LoadMoveUp();
            else if (file.IsDirectory)
                LoadDirectory(file);
            else
                LoadFile(file.FullName);
        }

        private void LoadMoveUp()
        {
            gContent.Children.Clear();
            gContent.Children.Add(new MoveUpPreview());
        }

        private void LoadDirectory(IDirectoryViewItem file)
        {
            gContent.Children.Clear();

            DirectoryPreview dirPreview = new DirectoryPreview();
            dirPreview.LoadDirectory(file.CreateFileSystem().GetDirectoryContent(file.FullName));

            gContent.Children.Add(dirPreview);
        }

        /// <summary>
        /// Load file to preview
        /// </summary>
        /// <param name="file">Path to file to preview</param>
        private void LoadFile(string path)
        {
            System.Diagnostics.Debug.Assert(Item != null, "Item not set");
            var fileType = FileTypeDiscover.DiscoverType(path);
            gContent.Children.Clear();
            switch (fileType)
            {
                case FileContentType.Unknown:
                    LoadText(path);
                    break;
                case FileContentType.Text:
                    LoadText(path);
                    break;
                case FileContentType.SourceCode:
                    LoadSourceCode(path);
                    break;
                case FileContentType.Pdf:
                    LoadPdf(path);
                    break;
                case FileContentType.Image:
                    LoadImage(path);
                    break;
                case FileContentType.Audio:
                    LoadAudio(path);
                    break;
                case FileContentType.Video:
                    LoadVideo(path);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Load video file
        /// </summary>
        /// <param name="path">Path to video file</param>
        private void LoadVideo(string path)
        {
            var vidPreview = new AudioVideoPreview();
            vidPreview.LoadVideo(path);
            gContent.Children.Add(vidPreview);
            gContent.Background = Brushes.Black;
        }

        /// <summary>
        /// Load audio file
        /// </summary>
        /// <param name="path">Path to audio file</param>
        private void LoadAudio(string path)
        {
            var vidPreview = new AudioVideoPreview();
            vidPreview.LoadAudio(path);
            gContent.Children.Add(vidPreview);
        }

        /// <summary>
        /// Load text file
        /// </summary>
        /// <param name="path">Path to text file</param>
        private void LoadText(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, Encoding.GetEncoding("windows-1250"), true);//QSTN: Is the best solution for encoding problem?
            TextPreview tPreview = new TextPreview();
            tPreview.Text = reader.ReadToEnd();
            gContent.Children.Add(tPreview);
        }

        /// <summary>
        /// Load source code
        /// </summary>
        /// <param name="path">Path to source code file</param>
        private void LoadSourceCode(string path)
        {
            var scPreview = new SourceCodePreview();
            scPreview.LoadSourceCodeFile(path);
            gContent.Children.Add(scPreview);
        }

        /// <summary>
        /// Load pdf file
        /// </summary>
        /// <param name="path">Path to pdf file</param>
        private void LoadPdf(string path)
        {
            PdfPreview pdfPreview = new PdfPreview();
            pdfPreview.LoadPdf(path);
            gContent.Children.Add(pdfPreview);
        }

        /// <summary>
        /// Load image file
        /// </summary>
        /// <param name="path">Path to image file</param>
        private void LoadImage(string path)
        {
            var imgPreview = new ImagePreview();
            imgPreview.LoadImage(path);
            gContent.Children.Add(imgPreview);
            gContent.Background = Brushes.Black;
        }
    }
}