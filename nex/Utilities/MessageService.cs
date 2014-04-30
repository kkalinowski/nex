using System.Windows;
using nex.Dialogs.MessageBoxDialogs;

namespace nex.Utilities
{
    public static class MessageService
    {
        public static void ShowInfo(string message)
        {
            Utility.FadeIn();
            MessageBox.Show(message, "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            Utility.FadeOut();
        }

        public static void ShowError(string message)
        {
            Utility.FadeIn();
            MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            Utility.FadeOut();
        }

        public static string ShowInput(string message)
        {
            Utility.FadeIn();
            var inputDialog = new InputDialog { Message = message };
            var res = inputDialog.ShowModalDialog();
            Utility.FadeOut();

            return res ? inputDialog.Input : null;
        }

        public static bool ShowQuestion(string message)
        {
            Utility.FadeIn();
            var res = MessageBox.Show(message, "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            Utility.FadeOut();

            return res == MessageBoxResult.Yes;
        }
    }
}
