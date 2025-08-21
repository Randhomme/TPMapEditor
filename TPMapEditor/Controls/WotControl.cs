using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class WotControl : DefaultControl
    {
        public WorldObject WorldObject { get; set; }
        public WotControl(WorldObject wot)
        {
            this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            this.RenderTransform = new RotateTransform();
            this.WorldObject = wot;
            var border = new Border()
            {
                Child = new Image() { Source = wot.Type.Image },
                BorderThickness = new System.Windows.Thickness(2.5),
                BorderBrush = new SolidColorBrush()
            };
            this.Child = border;
            this.Loaded += WotControl_Loaded;
            BindingOperations.SetBinding(this.RenderTransform, RotateTransform.AngleProperty, new Binding("ZRotation") { Source = this.WorldObject });
            BindingOperations.SetBinding(border.Child, Image.SourceProperty, new Binding("Type.Image") { Source = this.WorldObject });
            BindingOperations.SetBinding(border.BorderBrush, SolidColorBrush.ColorProperty, new Binding("Group.Color") { Source = this.WorldObject });
        }

        private void WotControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.GetBindingExpression(Canvas.LeftProperty).UpdateTarget();
            this.GetBindingExpression(Canvas.TopProperty).UpdateTarget();
        }

        private void WotControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this.WorldObject, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            this.SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this.WorldObject, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
            this.SizeChanged += WotControl_SizeChanged;
        }
    }
}
