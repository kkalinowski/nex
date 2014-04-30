using System.Windows;

namespace nex.Dialogs.MessageBoxDialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog
    {
        #region Props
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(InputDialog));

        public string Input { get; set; }
        #endregion

        #region Start
        public InputDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void InputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            tInput.Focus();
        }
        #endregion
    }
}
