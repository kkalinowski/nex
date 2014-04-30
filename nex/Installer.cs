using lib12.DependencyInjection;
using nex.Dialogs.RenameDialog;

namespace nex
{
    public static class Installer
    {
        public static void Install()
        {
            Instances.RegisterSingleton<MainView>();
            Instances.RegisterTransient<RenameDialog>();
        }
    }
}
