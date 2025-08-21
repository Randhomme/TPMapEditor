using System.Windows.Controls;

namespace TPMapEditor.Controls
{
    public class DefaultControl : Border
    {
        public DefaultControl()
        {
            BorderThickness = new System.Windows.Thickness(5);
            BorderBrush = System.Windows.Media.Brushes.Transparent;
        }
    }
}
