using System.Collections.Specialized;
using System.IO;
using System.Windows;
using lib12.DependencyInjection;

namespace nex.Commands
{
    [Singleton, WireUpAllProperties]
    public sealed class ClipboardCutCommand : DynamicCommand
    {
        #region Execute
        public override void Execute(object parameter)
        {
            var active = MainViewModel.ActiveDirectoryContainer.ActiveView;
            if (!active.FileSystem.IsWindowsFileSystem)
            {
                MessageBox.Show("To polecenie działa tylko w windows'owym systemie plików");
                return;
            }

            //get items to copy
            var items = MainViewModel.GetSelectedItems();
            if (items.Length == 0)
            {
                MessageBox.Show("Zaznacz obiekty do sskopiowania");
                return;
            }

            //get paths to copy from items
            var paths = new StringCollection();
            foreach (var item in items)
                paths.Add(item.FullName);

            //and here goes magic
            //set special binary data that indicates that file must be cut
            var moveEffect = new byte[] { 2, 0, 0, 0 };//cut
            var dropEffect = new MemoryStream();
            dropEffect.Write(moveEffect, 0, moveEffect.Length);

            //set data object with file's paths
            var data = new DataObject();
            data.SetFileDropList(paths);
            data.SetData("Preferred DropEffect", dropEffect);

            //set data object in clipboard
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);
        } 
        #endregion
    }
}
