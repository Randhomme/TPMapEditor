using System.Windows;
using System.Windows.Controls;

namespace TPMapEditor.DataTemplate
{
    public partial class RuleFieldTemplate
    {
        private void GroupUnitComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = (ComboBox)sender;
            if (combo.SelectedIndex == -1 && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }
    }
}
