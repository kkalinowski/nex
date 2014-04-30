using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using nex.Controls;
using nex.Controls.DirectoryViews;
using nex.DirectoryView;
using nex.Utilities.Serialization;

namespace nex
{
    public partial class MainView : ISerializationHelperUser
    {
        #region Const
        private const string pathLeft = @"D:\temp1\sync";
        private const string pathRight = @"D:\temp2";
        #endregion

        #region Fields
        private DirectoryViewContainer lastActive;//store last focused DV, use it only when standard method fail! 
        #endregion

        public MainView()
        {
            InitializeComponent();

            //QSTN: Why it must be here to show PlacesPanel and RightPanel?
            //dvLeft.Load();
            //dvRight.Load();

            //block Tab key on DVs
            KeyboardNavigation.SetTabNavigation(gDV, KeyboardNavigationMode.Cycle);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //keyboard shortcuts
            InputBindings.Add(new InputBinding(bFind.Command, new KeyGesture(Key.F, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(bClipboardCopy.Command, new KeyGesture(Key.C, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(bClipboardCut.Command, new KeyGesture(Key.X, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(bClipboardPaste.Command, new KeyGesture(Key.V, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(bRename.Command, new KeyGesture(Key.F2)));
            InputBindings.Add(new InputBinding(bView.Command, new KeyGesture(Key.F3)));
            InputBindings.Add(new InputBinding(bEdit.Command, new KeyGesture(Key.F4)));
            InputBindings.Add(new InputBinding(bCopy.Command, new KeyGesture(Key.F5)));
            InputBindings.Add(new InputBinding(bMove.Command, new KeyGesture(Key.F6)));
            InputBindings.Add(new InputBinding(bNewDir.Command, new KeyGesture(Key.F7)));
            InputBindings.Add(new InputBinding(bDelete.Command, new KeyGesture(Key.F8)));
            InputBindings.Add(new InputBinding(bQATBack.Command, new KeyGesture(Key.Left, ModifierKeys.Alt)));
            InputBindings.Add(new InputBinding(bQATForward.Command, new KeyGesture(Key.Right, ModifierKeys.Alt)));
        }

        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RibbonWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            //FIX: Collides with DV.SimpleSearch
            if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                dvLeft.SetFocusOnContent();
            else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                dvRight.SetFocusOnContent();
            if (e.Key == Key.Tab)
                ChangeActiveDV();
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.A)
                GetActiveDV().SelectAll();
            else
                e.Handled = false;
        }

        #region ActiveDV
        /// <summary>
        /// Gets the active DirectoryViewContainer
        /// </summary>
        /// <returns>Active DirectoryViewContainer</returns>
        private DirectoryViewContainer GetActiveDV()
        {

            if (dvLeft.IsActive)
                return dvLeft;
            else if (dvRight.IsActive)
                return dvRight;
            else
            {
                lastActive.SetFocusOnContent();
                return lastActive;
            }
        }

        /// <summary>
        /// Support event LostKeyboardFocus for both DVs - save last active dv
        /// </summary>
        private void ActiveDVChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            lastActive = (DirectoryViewContainer)sender;
        }

        /// <summary>
        /// Changes the active DirectoryViewContainer
        /// </summary>
        private void ChangeActiveDV()
        {
            if (dvLeft.IsActive)
                dvRight.SetFocusOnContent();
            else
                dvLeft.SetFocusOnContent();
        }

        /// <summary>
        /// Returns safe, unchanchable array of selected items
        /// </summary>
        /// <returns>Safe, unchanchable array of selected items</returns>
        private IDirectoryViewItem[] GetSelectedItems()
        {
            DirectoryViewContainer source = GetActiveDV();
            IDirectoryViewItem[] items = new IDirectoryViewItem[source.SelectedItemsCount];
            source.SelectedItems.CopyTo(items, 0);

            return items.Where(i => !i.IsMoveUp).ToArray();//OPT: Find better solution
        }
        #endregion

        #region ISerializationHelperUser Members

        public string SerializationKey
        {
            get
            {
                return "Main";
            }
        }

        public SerializationData GetDataToSave()
        {
            Dictionary<string, object> data = new Dictionary<string, object>(2);
            data.Add("dvLeft", dvLeft);
            data.Add("dvRight", dvRight);
            return new SerializationData(SerializationKey, data);
        }

        public void ApplyLoadedData()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }
    }
}