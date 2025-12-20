using System.Windows;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;

namespace TPMapEditor.DataTemplate
{
    public partial class GroupTemplate
    {
        private void EditGroupColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Group group)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "Group color", group.Color);
                if (cp.ShowDialog() == true)
                    group.Color = cp.NewColor;
            }
        }

        private void RemoveWorldObjectFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is WorldObject worldObject)
            {
                worldObject.Group = null;
            }
        }
    }
}
