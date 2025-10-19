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
        private int size;

        public MapSizeDialog(Window owner, int size) : base(owner)
        {
            this.size = size;
            InitializeComponent();
            DataContext = this;
        }

        private void SliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var sliderValue = SliderSize.Value;
            TextSize.Text = sliderValue + " x " + sliderValue;
        }
    }
}
