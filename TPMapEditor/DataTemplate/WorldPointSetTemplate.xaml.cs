using System.Windows;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;

namespace TPMapEditor.DataTemplate
{
    public partial class WorldPointSetTemplate
    {
        private void EditWorldPointSetColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is WorldPointSet worldPointSet)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "World point set color", worldPointSet.Color);
                if (cp.ShowDialog() == true)
                    worldPointSet.Color = cp.NewColor;
            }
        }
    }
}
