using System.Windows;
using System.Windows.Controls;
using TPMapEditor.Interfaces;

namespace TPMapEditor.DataTemplate
{
    public partial class MultiMovableMapObjectTemplate
    {
        private void SliderX_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.BeginAlignXTransformMapCommand(e.OldValue);
                vm.UpdateAlignTransformMapCommand();
                vm.EndAlignTransformMapCommand();
            }
        }

        private void SliderY_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.BeginAlignYTransformMapCommand(e.OldValue);
                vm.UpdateAlignTransformMapCommand();
                vm.EndAlignTransformMapCommand();
            }
        }

        private void SliderZ_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.BeginAlignZTransformMapCommand(e.OldValue);
                vm.UpdateAlignTransformMapCommand();
                vm.EndAlignTransformMapCommand();
            }
        }

        private void SliderX_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                slider.ValueChanged -= SliderX_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
                vm.BeginAlignXTransformMapCommand(vm.X);
            }
        }

        private void SliderY_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                slider.ValueChanged -= SliderY_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
                vm.BeginAlignYTransformMapCommand(vm.Y);
            }
        }

        private void SliderZ_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                slider.ValueChanged -= SliderZ_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
                vm.BeginAlignZTransformMapCommand(vm.Z);
            }
        }

        private void SliderX_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.EndAlignTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderX_ValueChanged_BeginEndCommand;
            }
        }

        private void SliderY_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.EndAlignTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderY_ValueChanged_BeginEndCommand;
            }
        }

        private void SliderZ_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.EndAlignTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderZ_ValueChanged_BeginEndCommand;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiMovableMapObject vm)
            {
                vm.UpdateAlignTransformMapCommand();
            }
        }
    }
}
