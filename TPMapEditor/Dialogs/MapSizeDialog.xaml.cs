using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for MapSizeDialog.xaml
    /// </summary>
    public partial class MapSizeDialog : DialogWindow
    {
        [ObservableProperty]
        private int size, zSize;
        [ObservableProperty]
        private double worldBuffer;

        public MapSizeDialog(Window owner, int size, int zSize, double worldBuffer) : base(owner)
        {
            this.size = size;
            this.zSize = zSize;
            this.worldBuffer = worldBuffer;
            InitializeComponent();
            DataContext = this;
        }

        private void SliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var sliderValue = SliderSize.Value;
            TextSize.Text = $"{sliderValue} x {sliderValue}";
        }

        private void SliderZSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var sliderZValue = SliderZSize.Value;
            TextZSize.Text = $"{sliderZValue} x {sliderZValue}";
        }

        private void SliderWorldBuffer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var sliderWorldBufferValue = SliderWorldBuffer.Value;
            TextWorldBuffer.Text = $"{sliderWorldBufferValue}";
        }
    }
}
