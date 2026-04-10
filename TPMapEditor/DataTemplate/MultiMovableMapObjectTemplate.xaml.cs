using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TPMapEditor.ViewModel;

namespace TPMapEditor.DataTemplate
{
    public partial class MultiMovableMapObjectTemplate
    {
        protected void SliderX_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignXTransformMapCommand();
                vm.UpdateAlignXTransformMapCommand();
                vm.EndAlignXTransformMapCommand();
            }
        }

        protected void SliderX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.UpdateAlignXTransformMapCommand();
            }
        }

        protected void SliderX_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignXTransformMapCommand();
                slider.ValueChanged -= SliderX_ValueChanged_BeginEndCommand;
                slider.ValueChanged += SliderX_ValueChanged;
            }
        }

        protected void SliderX_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.EndAlignXTransformMapCommand();
                slider.ValueChanged -= SliderX_ValueChanged;
                slider.ValueChanged += SliderX_ValueChanged_BeginEndCommand;
            }
        }

        protected void SliderY_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignYTransformMapCommand();
                vm.UpdateAlignYTransformMapCommand();
                vm.EndAlignYTransformMapCommand();
            }
        }

        protected void SliderY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.UpdateAlignYTransformMapCommand();
            }
        }

        protected void SliderY_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignYTransformMapCommand();
                slider.ValueChanged -= SliderY_ValueChanged_BeginEndCommand;
                slider.ValueChanged += SliderY_ValueChanged;
            }
        }

        protected void SliderY_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.EndAlignYTransformMapCommand();
                slider.ValueChanged -= SliderY_ValueChanged;
                slider.ValueChanged += SliderY_ValueChanged_BeginEndCommand;
            }
        }

        protected void SliderZ_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignZTransformMapCommand();
                vm.UpdateAlignZTransformMapCommand();
                vm.EndAlignZTransformMapCommand();
            }
        }

        protected void SliderZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.UpdateAlignZTransformMapCommand();
            }
        }

        protected void SliderZ_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.BeginAlignZTransformMapCommand();
                slider.ValueChanged -= SliderZ_ValueChanged_BeginEndCommand;
                slider.ValueChanged += SliderZ_ValueChanged;
            }
        }

        protected void SliderZ_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is MultiWorldObjectViewModel vm)
            {
                vm.EndAlignZTransformMapCommand();
                slider.ValueChanged -= SliderZ_ValueChanged;
                slider.ValueChanged += SliderZ_ValueChanged_BeginEndCommand;
            }
        }
    }
}
