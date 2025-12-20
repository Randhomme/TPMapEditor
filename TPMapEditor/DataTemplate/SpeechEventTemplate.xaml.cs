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
    public partial class SpeechEventTemplate
    {
        private void EditTextColor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is SpeechEvent speechEvent)
            {
                var cp = new ColorPicker(Window.GetWindow(element), "Speech event color", speechEvent.TextColor);
                if (cp.ShowDialog() == true)
                    speechEvent.TextColor = cp.NewColor;
            }
        }
    }
}
