using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;

namespace TPMapEditor.DataTemplate
{
    public partial class DataTemplate
    {
        public DataTemplate()
        {
            DataGridColumnRegistry.Register<WorldObject>(() => new List<DataGridColumn>
            {
                new DataGridTextColumn
                {
                    Header = "Type",
                    Binding = new Binding("Type")
                }
            });
        }

        private void EditPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement element && element.DataContext is Player player)
            {
                var cp = new ColorPicker(Window.GetWindow(element), player.Color);
                if (cp.ShowDialog() == true)
                    player.Color = cp.NewColor;
            }
        }

        private void EditGroupColor_Click(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement element && element.DataContext is Group group)
            {
                var cp = new ColorPicker(Window.GetWindow(element), group.Color);
                if (cp.ShowDialog() == true)
                    group.Color = cp.NewColor;
            }
        }

        private void RemoveWorldObjectFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement element && element.DataContext is WorldObject worldObject)
            {
                worldObject.Group = null;
            }
        }
    }
}
