using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class MapTextPointControl : DefaultControl
    {
        public MapTextPoint MapTextPoint { get; set; }

        public MapTextPointControl(MapTextPoint mapTextPoint)
        {
            this.MapTextPoint = mapTextPoint;
            var label = new Label() { Foreground = Brushes.White, FontSize = 30 };
            BindingOperations.SetBinding(label, Label.ContentProperty, new Binding("DisplayedText") { Source = MapTextPoint });
            this.Child = label;
            this.Loaded += MapTextPointControl_Loaded;
        }

        private void MapTextPointControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this.MapTextPoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            this.SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this.MapTextPoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
