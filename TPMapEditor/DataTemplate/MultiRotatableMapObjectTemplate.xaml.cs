using System.Windows;
using System.Windows.Controls;
using TPMapEditor.Interfaces;

namespace TPMapEditor.DataTemplate
{
    public partial class MultiRotatableMapObjectTemplate
    {
        private void SliderX_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinXFixTransformMapCommand();
                vm.UpdateSpinFixTransformMapCommand();
                vm.EndSpinFixTransformMapCommand();
            }
        }

        private void SliderY_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinYFixTransformMapCommand();
                vm.UpdateSpinFixTransformMapCommand();
                vm.EndSpinFixTransformMapCommand();
            }
        }

        private void SliderZ_ValueChanged_BeginEndCommand(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && (slider.IsFocused || slider.IsMouseOver) && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinZFixTransformMapCommand();
                vm.UpdateSpinFixTransformMapCommand();
                vm.EndSpinFixTransformMapCommand();
            }
        }

        private void SliderX_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinXFixTransformMapCommand();
                slider.ValueChanged -= SliderX_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
            }
        }

        private void SliderY_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinYFixTransformMapCommand();
                slider.ValueChanged -= SliderY_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
            }
        }

        private void SliderZ_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.BeginSpinZFixTransformMapCommand();
                slider.ValueChanged -= SliderZ_ValueChanged_BeginEndCommand;
                slider.ValueChanged += Slider_ValueChanged;
            }
        }

        private void SliderX_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.EndSpinFixTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderX_ValueChanged_BeginEndCommand;
            }
        }

        private void SliderY_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.EndSpinFixTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderY_ValueChanged_BeginEndCommand;
            }
        }

        private void SliderZ_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.EndSpinFixTransformMapCommand();
                slider.ValueChanged -= Slider_ValueChanged;
                slider.ValueChanged += SliderZ_ValueChanged_BeginEndCommand;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.DataContext is IMultiRotatableMapObject vm)
            {
                vm.UpdateSpinFixTransformMapCommand();
            }
        }
    }
}
