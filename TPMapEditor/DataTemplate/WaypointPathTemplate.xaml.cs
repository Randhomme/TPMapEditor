using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;

namespace TPMapEditor.DataTemplate
{
    public partial class WaypointPathTemplate
    {
        private void EditWaypointPathColor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is WaypointPath waypointPath)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "Waypoint path color", waypointPath.Color);
                if (cp.ShowDialog() == true)
                    waypointPath.Color = cp.NewColor;
            }
        }
    }
}
