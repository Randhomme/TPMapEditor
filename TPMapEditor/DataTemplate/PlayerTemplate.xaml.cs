using System.Windows;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;

namespace TPMapEditor.DataTemplate
{
    public partial class PlayerTemplate
    {
        private void EditPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Player player)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "Player color", player.Color);
                if (cp.ShowDialog() == true)
                    player.Color = cp.NewColor;
            }
        }
    }
}
