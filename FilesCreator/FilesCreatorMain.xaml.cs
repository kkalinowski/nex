using System.IO;
using System.Windows;
using System.Windows.Forms;
using System;
using System.Windows.Threading;

namespace FilesCreator
{
    /// <summary>
    /// Interaction logic for FilesCreatorMain1.xaml
    /// </summary>
    public partial class FilesCreatorMain : Window
    {
        public FilesCreatorMain()
        {
            InitializeComponent();
        }

        private void bChangeDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dirDialog = new FolderBrowserDialog();
            dirDialog.SelectedPath=@"D:\";
            if (dirDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tDir.Text = dirDialog.SelectedPath;
        }

        private void bCreate_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            byte[] buffer=new byte[100000];
            string path;
            int count=int.Parse(tCount.Text);

            rand.NextBytes(buffer);
            pbFilesCreation.Value = 0;
            pbFilesCreation.Maximum = count;

            for (int i = 0; i < count; i++)
            {
                path = Path.Combine(tDir.Text, Path.GetRandomFileName());
                using (FileStream file = File.Create(path))
                    file.Write(buffer, 0, buffer.Length);
                pbFilesCreation.Value++;
                if (i % 25 == 0)
                    pbFilesCreation.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate.Instance);
            }

            System.Windows.Forms.MessageBox.Show("Utworzono "+count+" plików w folderze "+tDir.Text);
        }
    }
}
