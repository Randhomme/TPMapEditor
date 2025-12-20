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
    public partial class WorldPolygonTemplate
    {
        private void EditWorldPolygonColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is WorldPolygon worldPolygon)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "World polygon color", worldPolygon.Color);
                if (cp.ShowDialog() == true)
                    worldPolygon.Color = cp.NewColor;
            }
        }
    }
}
