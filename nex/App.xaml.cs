using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Elysium;
using lib12.DependencyInjection;
using lib12.Serialization;
using lib12.IO;
using lib12.WPF.Extensions;

namespace nex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Const
        private static readonly string AppDataPath = Path.Combine(IOHelper.GetAppDataPath(), "nex");
        private static readonly string DataPath = Path.Combine(AppDataPath, "nex.data");
        #endregion

        private bool exitWithError = false;

        private void AppStartup(object sender, StartupEventArgs e)
        {
            IOHelper.CreateDirectoryIfNotExist(AppDataPath);
            if (File.Exists(DataPath))
                SerializationHelper.Current.Load(DataPath);

            Installer.Install();
            Instances.Get<MainView>().Show();

            ThemeManager.ApplyTheme(this, Theme.Light, (SolidColorBrush)Application.Current.FindBrush("AccentColor"),
                (SolidColorBrush)Application.Current.FindBrush("FontColor"));
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            //save data
            if (!exitWithError)
                SerializationHelper.Current.Save(DataPath);
        }

        public static object GetRes(string key)
        {
            return Current.Resources[key];
        }

        public static Style GetStyle(string key)
        {
            return (Style)Current.Resources[key];
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string msg = string.Format("Wystąpił błąd - {0}.\n W metodzie {1}", e.Exception.Message, e.Exception.StackTrace);
            MessageBox.Show(msg);

            exitWithError = true;
            e.Handled = true;
            Shutdown(-1);
        }
    }
}