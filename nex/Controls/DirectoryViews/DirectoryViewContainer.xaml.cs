using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using lib12.DependencyInjection;
using lib12.WPF.Converters;
using nex.DirectoryView;
using nex.FileSystem;
using nex.FileSystem.Windows;
using nex.HistoryLogic;
using nex.Operations;
using nex.Utilities;
using nex.Utilities.Serialization;

namespace nex.Controls.DirectoryViews
{
    /// <summary>
    /// This class displays directories and its content
    /// </summary>
    [Transient]
    public partial class DirectoryViewContainer : UserControl, IDirectoryView, ISerializationHelperUser
    {
        //implement interface IDirectoryView for easier cooperetion in MainView
        //TODO: Header glow on mouse over shouldn't be everywhere
        public event EventHandler ItemExecuted;

        #region Fields
        private bool isActiveFolderFavorite; 
        #endregion

        #region Props
        [WireUp]
        public MainView Main { get; set; }

        /// <summary>
        /// Get the dir content
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                return ActiveView.Items;
            }
        }
        /// <summary>
        /// Get or set path to currently displayed directory
        /// </summary>
        public string DisplayPath
        {
            get
            {
                return ActiveView.DisplayPath;
            }
        }
        public bool IsMoveUpSelected
        {
            get
            {
                return ActiveView.IsMoveUpSelected;
            }
        }
        public bool IsActive
        {
            get
            {
                return ActiveView.IsActive;
            }
        }
        public bool IsOneFileSelected
        {
            get
            {
                return ActiveView.IsOneFileSelected;
            }
        }
        public IDirectoryView UnchangebleActiveView
        {
            get
            {
                TabItem active = (TabItem)tcViews.SelectedItem;
                return (IDirectoryView)active.Content;
            }
        }
        #endregion

        #region DependProps
        public bool SimpleSearch
        {
            get
            {
                return (bool)GetValue(SimpleSearchProperty);
            }
            set
            {
                SetValue(SimpleSearchProperty, value);
            }
        }

        public static readonly DependencyProperty SimpleSearchProperty =
            DependencyProperty.Register("SimpleSearch", typeof(bool), typeof(DirectoryViewContainer));

        public IDirectoryView ActiveView
        {
            get
            {
                return (IDirectoryView)GetValue(ActiveViewProperty);
            }
            private set
            {
                SetValue(ActiveViewProperty, value);
            }
        }

        private static readonly DependencyPropertyKey ActiveViewPropertyKey =
            DependencyProperty.RegisterReadOnly("ActiveView", typeof(IDirectoryView), typeof(DirectoryViewContainer), new PropertyMetadata());

        public static readonly DependencyProperty ActiveViewProperty = ActiveViewPropertyKey.DependencyProperty;
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        public DirectoryViewContainer()
        {
            InitializeComponent();
        }

        public void Load()
        {
            SerializationHelper.RegisterUser(this);

            if (SerializationHelper.DataLoaded)
                ApplyLoadedData();
            else
            {
                //AddNewTab(new DetailsView(), null);
                tcViews.SelectedIndex = 0;//before showing selectedindex=-1
            }

            ActiveView.ItemExecuted += ActiveView_ItemExecuted;

            //Binding must be set here
            Binding fullPathBinding = new Binding("ActiveView.FileSystem.FullPath");
            fullPathBinding.Source = this;
            fullPathBinding.Converter = new SubstringConverter { MaxLength = 50 };
            tPath.SetBinding(TextBlock.TextProperty, fullPathBinding);

            Binding rootIconBind = new Binding("ActiveView.FileSystem.Root.Icon");
            rootIconBind.Source = this;
            iRoot.SetBinding(Image.SourceProperty, rootIconBind);

            Binding rootTakenPlaceBind = new Binding("ActiveView.FileSystem.Root.TakenPercent");
            rootTakenPlaceBind.Source = this;
            rootTakenPlaceBind.Mode = BindingMode.OneWay;
            pbTakenPlaceRoot.SetBinding(ProgressBar.ValueProperty, rootTakenPlaceBind);
        }

        protected void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Enter && SelectedItem != null && SelectedItem.IsDirectory)
                LoadSelectedDir();
            else if (e.Key == Key.Enter && SelectedItem != null && SelectedItem.IsMoveUp && !PathExt.IsDriveRoot(DisplayPath))
                MoveUp();
            else if (e.Key == Key.Back && tSearch.Text != string.Empty)
                tSearch.Text = tSearch.Text.Remove(tSearch.Text.Length - 1);
            else if (e.Key == Key.Back && !PathExt.IsDriveRoot(DisplayPath))
                MoveUp();
            else if (e.Key == Key.Enter && IsOneFileSelected)
                ExecuteSelectedFile();
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.T))
                AddNewTab();
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.KeyboardDevice.IsKeyDown(Key.W))
                TabCloseClicked(null, null);
            else
            {
                char keyID = Utility.ConvertKeyToChar(e.Key);
                if (keyID != char.MinValue)
                {
                    if (char.IsLetterOrDigit(keyID))
                        tSearch.AppendText(keyID.ToString());
                    else if (e.Key == Key.Space)//TODO: Handle space
                        tSearch.AppendText(" ");
                    EnableSimpleSearch();
                }
                else
                    e.Handled = false;
            }
        }

        private void EnableSimpleSearch()
        {
            SimpleSearch = true;
            ItemCollection items = ActiveView.Items;
            foreach (IDirectoryViewItem item in items)
                item.IsMatchingCriteria = item.Name.ToLower().Contains(tSearch.Text);

            //MoveUp must be first
            using (items.DeferRefresh())
            {
                SortDescription moveUpSort = new SortDescription("IsMoveUp", ListSortDirection.Descending);
                items.SortDescriptions.Add(moveUpSort);

                //sort on IsMatchingCriteria
                SortDescription matchSort = new SortDescription("IsMatchingCriteria", ListSortDirection.Descending);
                items.SortDescriptions.Add(matchSort);

                //sort on Name
                SortDescription nameSort = new SortDescription("Name", ListSortDirection.Ascending);
                items.SortDescriptions.Add(nameSort);
            }
        }

        private void DisableSimpleSearch()
        {
            if (SimpleSearch)
            {
                SimpleSearch = false;
                ActiveView.Items.SortDescriptions.Clear();
                tSearch.Clear();
            }
        }

        private void AddNewTab()
        {
            //create new TabItem & apply style
            TabItem ti = new TabItem();
            ti.Style = (Style)Resources["CloseableTabItem"];

            IDirectoryView view = (IDirectoryView)Activator.CreateInstance(ActiveView.GetType());
            UserControl viewCtrl = (UserControl)view;
            viewCtrl.Height = double.NaN;//same as Auto in XAML
            viewCtrl.VerticalAlignment = VerticalAlignment.Stretch;
            viewCtrl.Width = double.NaN;
            viewCtrl.HorizontalAlignment = HorizontalAlignment.Stretch;

            //events
            view.ItemExecuted += ActiveView_ItemExecuted;

            //load last dir to new details view
            view.LoadDir(DisplayPath, true);

            //set content listview as content of tabitem
            ti.Content = view;

            //add tabitem to tabcontrol
            tcViews.Items.Insert(tcViews.Items.Count - 1, ti);
            tcViews.SelectedIndex = tcViews.Items.Count - 2;//QSTN: Why new tab must be selected, to gain header?

            //bind header
            Binding bind = new Binding("FileSystem.DirectoryName");
            bind.Source = view;
            ti.SetBinding(TabItem.HeaderProperty, bind);
        }

        private void AddNewTab(IDirectoryView view, FileSystemBase fileSystem)
        {
            //create new TabItem & apply style
            TabItem ti = new TabItem();
            ti.Style = (Style)Resources["CloseableTabItem"];

            UserControl viewCtrl = (UserControl)view;
            viewCtrl.Height = double.NaN;//same as Auto in XAML
            viewCtrl.VerticalAlignment = VerticalAlignment.Stretch;
            viewCtrl.Width = double.NaN;
            viewCtrl.HorizontalAlignment = HorizontalAlignment.Stretch;

            //events
            view.ItemExecuted += ActiveView_ItemExecuted;

            //set file system
            if (fileSystem != null)
                view.ChangeFileSystem(fileSystem);

            //set content listview as content of tabitem
            ti.Content = view;

            //add tabitem to tabcontrol
            tcViews.Items.Insert(tcViews.Items.Count - 1, ti);
            tcViews.SelectedIndex = tcViews.Items.Count - 2;//QSTN: Why new tab must be selected, to gain header?

            //bind header
            Binding bind = new Binding("FileSystem.DirectoryName");
            bind.Source = view;
            ti.SetBinding(TabItem.HeaderProperty, bind);
        }

        /// <summary>
        /// Not strait road to notify on change tabs
        /// </summary>
        private void tcViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //OPT: Focus item on new selected tab after closing another one
            if (e.Source is TabControl)
            {
                ActiveViewChangeNotify();
                DisableSimpleSearch();
            }
        }

        private void ActiveViewChangeNotify()
        {
            TabItem active = (TabItem)tcViews.SelectedItem;
            if (active != null)
                SetValue(ActiveViewPropertyKey, active.Content);
        }

        private void TabCloseClicked(object sender, RoutedEventArgs e)
        {
            if (tcViews.Items.Count > 2)
            {
                //TODO: Write a class to quick find ancestor/parent
                DependencyObject stack = VisualTreeHelper.GetParent((DependencyObject)sender);
                DependencyObject contentPresenter = VisualTreeHelper.GetParent(stack);
                DependencyObject border = VisualTreeHelper.GetParent(contentPresenter);
                DependencyObject grid = VisualTreeHelper.GetParent(border);
                DependencyObject tabItem = VisualTreeHelper.GetParent(grid);
                tcViews.Items.Remove(tabItem);
                //OPT: When tab is closed I cannot gain focus for new active tab items
                tcViews.SelectedIndex = tcViews.Items.Count - 2;
                //SetFocusOnContent();
            }
        }

        private void ExecuteSelectedFile()
        {
            DisableSimpleSearch();
            ProcessStartInfo psi = new ProcessStartInfo(SelectedItem.FullName);
            Process.Start(psi);
        }

        private void ExecuteFile(IDirectoryViewItem item)
        {
            DisableSimpleSearch();
            ProcessStartInfo psi = new ProcessStartInfo(item.FullName);
            Process.Start(psi);
        }

        /// <summary>
        /// Loads directory content into control
        /// </summary>
        /// <param name="dir">Path to directory to show</param>
        public void LoadDir(string dir, bool saveInHistory)
        {
            DisableSimpleSearch();
            ActiveView.LoadDir(dir, saveInHistory);
        }

        /// <summary>
        /// Loads currently selected directory
        /// </summary>
        public void LoadSelectedDir()
        {
            DisableSimpleSearch();
            ActiveView.LoadSelectedDir();
        }

        /// <summary>
        /// Set focous on item no. zero
        /// </summary>
        public void SetFocusOnContent()
        {
            //ActiveView.SetFocusOnContent();
        }

        private void ActiveView_ItemExecuted(object sender, EventArgs e)
        {
            IDirectoryViewItem item = (IDirectoryViewItem)sender;

            Debug.Assert(item != null, "Empty call");

            if (item.IsDirectory)
                LoadDir(item.FullName, true);
            else if (item.IsMoveUp)
                MoveUp();
            else
                ExecuteFile(item);
        }

        public IDirectoryViewItem SelectedItem
        {
            get
            {
                return ActiveView.SelectedItem;
            }
        }

        public IList SelectedItems
        {
            get
            {
                return ActiveView.SelectedItems;
            }
        }

        public int SelectedItemsCount
        {
            get
            {
                return ActiveView.SelectedItems.Count;
            }
        }

        public void RefreshView()
        {
            ActiveView.LoadDir(DisplayPath, false);
        }

        #region IDirectoryView
        public FileSystemBase FileSystem
        {
            get
            {
                return ActiveView.FileSystem;
            }
        }

        public History<IDirectoryViewItem> History
        {
            get
            {
                return ActiveView.History;
            }
            set
            {
                ActiveView.History = value;
            }
        }

        public void SelectAll()
        {
            ActiveView.SelectAll();
        }

        public void UnselectAll()
        {
            ActiveView.UnselectAll();
        }

        /// <summary>
        /// Move up one place in the directory hierarchy
        /// </summary>
        public void MoveUp()
        {
            DisableSimpleSearch();
            ActiveView.MoveUp();
        }

        public void MoveBack()
        {
            ActiveView.MoveBack();
        }

        public void MoveForward()
        {
            ActiveView.MoveForward();
        }

        public string DirectoryName
        {
            get
            {
                return ActiveView.DirectoryName;
            }
        }

        #endregion

        private void tSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tSearch.Text == string.Empty)
                DisableSimpleSearch();
        }

        /// <summary>
        /// Support the event - clicked FileSystemRoot object - return to root main directory
        /// </summary>
        private void RootClicked(object sender, RoutedEventArgs e)
        {
            FileSystem.LoadDefaultDirectory();
        }

        private void DropCopy(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            IDirectoryViewItem[] files = (IDirectoryViewItem[])item.Tag;

            CopyOperation copy = new CopyOperation(files, DisplayPath, FileSystem, FileSystem);
            //Main.SupportOperation(copy);
        }

        private void DropMove(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            IDirectoryViewItem[] files = (IDirectoryViewItem[])item.Tag;

            MoveOperation move = new MoveOperation(files, DisplayPath, new WindowsFileSystem(), FileSystem);
            //Main.SupportOperation(move);
        }

        public void ChangeFileSystem(FileSystemBase fileSystem)
        {
            DisableSimpleSearch();
            ActiveView.ChangeFileSystem(fileSystem);
        }

        private void _DirectoryViewContainer_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
                DisableSimpleSearch();//During searching, when changing active DirectoryView
        }

        private void tiAdd_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            AddNewTab();
        }

        #region ISerializationHelperUser Members

        public string SerializationKey { get; set; }

        public SerializationData GetDataToSave()
        {
            Dictionary<string, object> data = new Dictionary<string, object>(2 * tcViews.Items.Count);
            data.Add("Count", tcViews.Items.Count - 1);
            data.Add("ActiveView", tcViews.SelectedIndex);

            for (int i = 0; i < tcViews.Items.Count - 1; i++)
            {
                TabItem item = tcViews.Items[i] as TabItem;
                data.Add(i + "Type", item.Content.GetType());
                data.Add(i + "FileSystem", ((IDirectoryView)item.Content).FileSystem);
            }

            return new SerializationData(SerializationKey, data);
        }

        public void ApplyLoadedData()
        {
            SerializationData data = SerializationHelper.GetData(SerializationKey);
            Dictionary<string, object> dict = data.Data;
            int count = (int)dict["Count"];

            for (int i = 0; i < count; i++)
            {
                Type viewType = (Type)dict[i + "Type"];
                IDirectoryView view = (IDirectoryView)Activator.CreateInstance(viewType);

                FileSystemBase fileSystem = (FileSystemBase)dict[i + "FileSystem"];

                //AddNewTab(view, fileSystem);
            }

            tcViews.SelectedIndex = (int)dict["ActiveView"];
        }

        #endregion

        private void _DirectoryViewContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetFocusOnContent();
        }

        #region Events
        private void _DirectoryViewContainer_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                dropMenu.IsOpen = true;
        }
        #endregion
    }
}