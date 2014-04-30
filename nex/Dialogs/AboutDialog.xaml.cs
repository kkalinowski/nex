using System.Windows;
using System.Windows.Input;

namespace nex.Controls
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            tInfo.Text = "Program nex \n" +
                         "Stworzony przy użyciu platformy \n.Net w wersji 3.5 SP1 dla systemu Windows.\n" +
                         "Program wykonany na pracę magisterską \nprzez Krzysztofa Kalinowskiego\n" +
                         "Kraków 2010" +
                         "\n\nPodziękowania dla:\nPromotora - dr hab. Piotra Białasa" +
                         "\nPawła Sitarza\nAleksandra Sprężynę\nRadosława Polańskiego";
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}