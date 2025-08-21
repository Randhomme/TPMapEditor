using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class WorldPointControl : DefaultControl
    {
        public WorldPoint WorldPoint { get; set; }

        public WorldPointControl(WorldPoint worldPoint)
        {
            this.WorldPoint = worldPoint;
            this.Child = new Image() { Source = new BitmapImage(new System.Uri("pack://application:,,,/Images/Arrow.png")) };
            this.Background = new SolidColorBrush();
            this.RenderTransform = new RotateTransform();
            this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            BindingOperations.SetBinding(Background, SolidColorBrush.ColorProperty, new Binding("Color") { Source = this.WorldPoint });
            BindingOperations.SetBinding(this.RenderTransform, RotateTransform.AngleProperty, new Binding("ZRotation") { Source = this.WorldPoint });
            this.Loaded += WorldPointControl_Loaded;
        }

        private void WorldPointControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this.WorldPoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            this.SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this.WorldPoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
