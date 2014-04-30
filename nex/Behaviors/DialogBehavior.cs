using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ElysiumWindow = Elysium.Controls.Window;

namespace nex.Behaviors
{
    public sealed class DialogBehavior : Behavior<ElysiumWindow>
    {
        #region Fields
        private Button bOk;
        private Button bCancel;
        #endregion

        #region Attached
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            FindButtons(AssociatedObject);
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            bOk.Click += bOk_Click;
            bCancel.Click += bCancel_Click;
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        private void FindButtons(DependencyObject parent)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var button = child as Button;
                if (button == null)
                {
                    FindButtons(child);
                }
                else if (button.IsDefault)
                {
                    bOk = button;
                    if (bCancel != null)
                        return;
                }
                else if (button.IsCancel)
                {
                    bCancel = button;
                    if (bOk != null)
                        return;
                }
            }
        }
        #endregion

        #region Detached
        protected override void OnDetaching()
        {
            base.OnDetaching();
            bOk.Click -= bOk_Click;
            bCancel.Click -= bCancel_Click;
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }
        #endregion

        #region Events support
        void bOk_Click(object sender = null, RoutedEventArgs e = null)
        {
            AssociatedObject.DialogResult = true;
            AssociatedObject.Close();
        }

        void bCancel_Click(object sender = null, RoutedEventArgs e = null)
        {
            AssociatedObject.DialogResult = false;
            AssociatedObject.Close();
        }

        void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                bOk_Click();
            else if (e.Key == Key.Escape)
                bCancel_Click();
        }
        #endregion
    }
}