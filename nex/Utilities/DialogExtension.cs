using lib12.Extensions;
using Dialog = Elysium.Controls.Window;

namespace nex.Utilities
{
    public static class DialogExtension
    {
        public static bool ShowModalDialog(this Dialog dialog)
        {
            Utility.FadeIn();
            var res = dialog.ShowDialog();
            Utility.FadeOut();

            return res.IsTrue();
        }
    }
}
