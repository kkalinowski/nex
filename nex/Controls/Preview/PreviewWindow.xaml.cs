using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using lib12.Core;
using lib12.DependencyInjection;
using lib12.WPF.Extensions;
using nex.DirectoryView;
using nex.FileSystem;
using nex.FileSystem.Windows;

namespace nex.Controls.Preview
{
    [Transient]
    public partial class PreviewWindow
    {
        #region Fields
        private bool fullscreen = false;
        #endregion

        #region Props
        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }
        public static readonly DependencyProperty ShowControlsProperty =
            DependencyProperty.Register("ShowControls", typeof(bool), typeof(PreviewWindow));
        #endregion

        #region ctor
        public PreviewWindow(bool fullscreen)
        {
            InitializeComponent();
            if (fullscreen)
                bFullscreen_Click(null, null);
        }
        #endregion

        public void LoadFile(IDirectoryViewItem file)
        {
            pvContainer.Item = file;
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (!fullscreen)
            {
                WindowStyle = WindowStyle.None;
                Topmost = true;
                WindowState = WindowState.Maximized;
                Background = Brushes.Transparent;
                bFullscreen.ToolTip = "Wyłącz pełny ekran";
                fullscreen = true;
            }
            else
            {
                WindowStyle = WindowStyle.ToolWindow;
                WindowState = WindowState.Normal;
                Background = Application.Current.FindBrush("AccentColor");
                bFullscreen.ToolTip = "Pełny ekran";
                Topmost = false;
                fullscreen = false;
            }
        }

        private void bPrev_Click(object sender, RoutedEventArgs e)
        {
            var directoryContent = Directory.GetFiles(PathExt.GetDirectoryName(pvContainer.Item.FullName, pvContainer.Item.IsWindowsFile));
            var itemIndex = Array.IndexOf(directoryContent, pvContainer.Item.FullName);
            var nextItem = directoryContent[Math2.Prev(itemIndex, directoryContent.Length)];
            LoadFile(new WindowsFile(new FileInfo(nextItem)));
        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            var directoryContent = Directory.GetFiles(PathExt.GetDirectoryName(pvContainer.Item.FullName, pvContainer.Item.IsWindowsFile));
            var itemIndex = Array.IndexOf(directoryContent, pvContainer.Item.FullName);
            var nextItem = directoryContent[Math2.Next(itemIndex, directoryContent.Length)];
            LoadFile(new WindowsFile(new FileInfo(nextItem)));
        }

        private void PreviewWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Left)
                bPrev_Click(null, null);
            else if (e.Key == Key.Right)
                bNext_Click(null, null);
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) && e.KeyboardDevice.IsKeyDown(Key.Enter))
                bFullscreen_Click(null, null);

            this.Focus();
        }

        private void PreviewWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            ShowControls = mousePosition.Y < 100;
        }
    }
}