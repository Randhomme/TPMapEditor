using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class PlayerControl : DefaultControl
    {
        public Player Player { get; set; }
        public PlayerControl(Player player, ImageSource source)
        {
            Player = player;
            var childGrid = new Grid();
            childGrid.Children.Add(new Ellipse()
            {
                Fill = new ImageBrush(new BitmapImage(new System.Uri("pack://application:,,,/Images/AlphaBg.png")))
                {
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = new System.Windows.Rect(0, 0, 10, 10),
                    TileMode = TileMode.Tile
                }
            });
            var bgColorEllipse = new Ellipse()
            {
                Fill = new SolidColorBrush(),
            };
            BindingOperations.SetBinding(bgColorEllipse.Fill, SolidColorBrush.ColorProperty, new Binding("Color") { Source = Player });
            childGrid.Children.Add(bgColorEllipse);
            childGrid.Children.Add(new Image() { Source = source, Margin = new System.Windows.Thickness(5) });
            RenderTransform = new RotateTransform();
            RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            BindingOperations.SetBinding(RenderTransform, RotateTransform.AngleProperty, new Binding("ZRotation") { Source = Player });
            Child = childGrid;
            Loaded += PlayerControl_Loaded;
        }

        private void PlayerControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetBinding(Canvas.LeftProperty, new Binding("X") { Source = Player, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            SetBinding(Canvas.TopProperty, new Binding("Y") { Source = Player, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
