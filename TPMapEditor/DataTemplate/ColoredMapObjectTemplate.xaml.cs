using System.Windows;
using TPMapEditor.Dialogs;
using TPMapEditor.Interfaces;

namespace TPMapEditor.DataTemplate
{
    public partial class ColoredMapObjectTemplate
    {
        private void EditColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is IColoredMapObject coloredMapObject)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "Edit color", coloredMapObject.Color);
                if (cp.ShowDialog() == true)
                    coloredMapObject.Color = cp.NewColor;
            }
        }
    }
}
