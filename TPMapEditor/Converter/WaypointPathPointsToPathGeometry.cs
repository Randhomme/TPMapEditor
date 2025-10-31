using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class WaypointPathPointsToPathGeometry : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<WaypointPathPoint> points && points.Count > 0)
            {
                var pathGeometry = new PathGeometry();

                var pathFigure = new PathFigure();
                pathGeometry.Figures.Add(pathFigure);

                BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = points[0].Point });

                for (int i = 1; i < points.Count; i++)
                {
                    var lineSegment = new LineSegment();
                    BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, new Binding("Point") { Source = points[i].Point });
                    pathFigure.Segments.Add(lineSegment);
                }

                return pathGeometry;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
