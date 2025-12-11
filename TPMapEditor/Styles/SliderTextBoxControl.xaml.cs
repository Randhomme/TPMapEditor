using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TPMapEditor.Styles
{
    public partial class SliderTextBoxControl : ResourceDictionary
    {
        private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
                slider.Focus();
        }
    }
}
