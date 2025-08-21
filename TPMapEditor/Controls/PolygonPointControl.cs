using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class PolygonPointControl : DefaultControl
    {
        public PolygonControl PolygonControl { get; }
        public Point2 Point { get; }
        public PolygonPointControl(PolygonControl polygonControl, WorldPolygon polygon, Point2 point)
        {
            this.PolygonControl = polygonControl;
            this.Point = point;
            this.Width = this.Height = 30;
            this.Background = new SolidColorBrush();
            BindingOperations.SetBinding(Background, SolidColorBrush.ColorProperty, new Binding("Color") { Source = polygon });
            this.Loaded += PolygonPointControl_Loaded;
        }

        private void PolygonPointControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this.Point, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            this.SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this.Point, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
