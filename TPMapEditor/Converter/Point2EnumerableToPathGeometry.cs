using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class Point2EnumerableToPathGeometry : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Point2> points)
            {
                var count = points.Count();
                var pathGeometry = new PathGeometry();
                if (count == 0)
                    return pathGeometry;

                var pathFigure = new PathFigure();
                pathGeometry.Figures.Add(pathFigure);

                BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = points.First() });

                foreach(var p in points.Skip(1))
                {
                    var lineSegment = new LineSegment();
                    BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, new Binding("Point") { Source = p });
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
