using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class PathPointControl : DefaultControl
    {
        public PathControl PathControl { get; }
        public Point3 Point { get; }
        public PathPointControl(PathControl pathControl, WaypointPath path, Point3 point)
        {
            PathControl = pathControl;
            Point = point;
            Width = Height = 30;
            Background = new SolidColorBrush();
            BindingOperations.SetBinding(Background, SolidColorBrush.ColorProperty, new Binding("Color") { Source = path });
            Loaded += PathPointControl_Loaded;
        }

        private void PathPointControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetBinding(Canvas.LeftProperty, new Binding("X") { Source = Point, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            SetBinding(Canvas.TopProperty, new Binding("Y") { Source = Point, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
