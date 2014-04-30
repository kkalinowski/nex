using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using lib12.Collections;
using lib12.DependencyInjection;
using lib12.WPF.Core;
using lib12.WPF.EventTranscriptions;
using nex.Controls.DirectoryViews;
using nex.DirectoryView;

namespace nex
{
    [Singleton]
    public sealed class MainViewModel : NotifyingObject
    {
        #region Props
        [WireUp]
        public MainView MainView { get; set; }
        [WireUp]
        public OperationsManager OperationsManager { get; set; }
        public DirectoryViewContainerViewModel LeftDirectoryContainerViewModel { get; private set; }
        public DirectoryViewContainerViewModel RightDirectoryContainerViewModel { get; private set; }

        public ICommand KeyboardCommand { get; private set; }
        public ICommand MouseCommand { get; private set; }

        private DirectoryViewContainerViewModel activeDirectoryContainer;
        public DirectoryViewContainerViewModel ActiveDirectoryContainer
        {
            get { return activeDirectoryContainer; }
            set
            {
                if (ActiveDirectoryContainer == value)
                    return;

                if (activeDirectoryContainer != null)
                {
                    activeDirectoryContainer.IsActive = false;
                    InActiveDirectoryContainer = activeDirectoryContainer;
                }
                activeDirectoryContainer = value;
                activeDirectoryContainer.IsActive = true;
                OnPropertyChanged("ActiveDirectoryContainer");
            }
        }

        private DirectoryViewContainerViewModel inActiveDirectoryContainer;
        public DirectoryViewContainerViewModel InActiveDirectoryContainer
        {
            get { return inActiveDirectoryContainer; }
            set
            {
                inActiveDirectoryContainer = value;
                OnPropertyChanged("InActiveDirectoryContainer");
            }
        }

        public DirectoryViewModel ActiveDirectoryView
        {
            get
            {
                return ActiveDirectoryContainer != null ? ActiveDirectoryContainer.ActiveView : null;
            }
        }
        #endregion

        #region ctor
        public MainViewModel(DirectoryViewContainerViewModel leftDirectoryContainerViewModel, DirectoryViewContainerViewModel rightDirectoryContainerViewModel)
        {
            ActiveDirectoryContainer = LeftDirectoryContainerViewModel = leftDirectoryContainerViewModel;
            InActiveDirectoryContainer = RightDirectoryContainerViewModel = rightDirectoryContainerViewModel;
            LeftDirectoryContainerViewModel.SerializationKey = "LeftDirectoryContainer";
            LeftDirectoryContainerViewModel.LoadDirectories();
            RightDirectoryContainerViewModel.SerializationKey = "RightDirectoryContainer";
            RightDirectoryContainerViewModel.LoadDirectories();

            KeyboardCommand = new DelegateCommand<EventTranscriptionParameter<KeyEventArgs>>(ExecuteKeyboard);
            MouseCommand = new DelegateCommand<EventTranscriptionParameter<MouseButtonEventArgs>>(ExecuteMouse);
        }
        #endregion

        #region Event support
        private void ExecuteKeyboard(EventTranscriptionParameter<KeyEventArgs> parameter)
        {
            if (parameter.EventArgs.Key == Key.D1 || parameter.EventArgs.Key == Key.NumPad1)
                ActiveDirectoryContainer = LeftDirectoryContainerViewModel;
            else if (parameter.EventArgs.Key == Key.D2 || parameter.EventArgs.Key == Key.NumPad2)
                ActiveDirectoryContainer = RightDirectoryContainerViewModel;
            if (parameter.EventArgs.Key == Key.Tab)
                ActiveDirectoryContainer = ActiveDirectoryContainer == LeftDirectoryContainerViewModel ? RightDirectoryContainerViewModel : LeftDirectoryContainerViewModel;
        }

        private void ExecuteMouse(EventTranscriptionParameter<MouseButtonEventArgs> parameter)
        {
            var directoryContainer = (DirectoryViewContainer)parameter.Sender;
            var vm = directoryContainer.DataContext;
            if (vm == LeftDirectoryContainerViewModel)
                ActiveDirectoryContainer = LeftDirectoryContainerViewModel;
            else
                ActiveDirectoryContainer = RightDirectoryContainerViewModel;
        }
        #endregion

        public IDirectoryViewItem[] GetSelectedItems()
        {
            return ActiveDirectoryContainer.ActiveView.SelectedItems.Cast<IDirectoryViewItem>().Where(i => !i.IsMoveUp).ToArray();
        }

        private void _Main_Closing(object sender, CancelEventArgs e)
        {
            if (OperationsManager.AreAnyOperationsOngoing)
            {
                var res = MessageBox.Show("Wykonywane są operacje, czy przerwać ich wykonywanie?", "Zamykanie programu...", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    OperationsManager.Pending.ForEach(x => x.Cancel());
                    e.Cancel = false;
                }
                else
                    e.Cancel = true;
            }
            else
                e.Cancel = false;
        }
    }
}
