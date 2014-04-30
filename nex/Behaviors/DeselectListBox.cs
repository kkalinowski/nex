using System.Windows.Controls;
using System.Windows.Interactivity;

namespace nex.Behaviors
{
    public sealed class DeselectListBox : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssociatedObject.UnselectAll();
        }
    }
}
