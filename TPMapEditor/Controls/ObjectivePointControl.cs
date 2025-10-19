using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.Converter;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class ObjectivePointControl : DefaultControl
    {
        public ObjectivePoint ObjectivePoint { get; set; }

        public ObjectivePointControl(ObjectivePoint objectivePoint)
        {
            this.ObjectivePoint = objectivePoint;
            this.Child = new Image() { Source = new BitmapImage(new System.Uri("pack://application:,,,/Images/ObjectivePoint.png")) };
            this.Loaded += ObjectivePointControl_Loaded;
        }

        private void ObjectivePointControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this.ObjectivePoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new XConverter(this) });
            this.SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this.ObjectivePoint, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay, Converter = new YConverter(this) });
        }
    }
}
