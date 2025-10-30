using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class WaypointPathPointToPointCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<WaypointPathPoint> points)
            {
                var collection = new PointCollection(points.Select(p => new Point(p.X, -p.Y)));
                return collection;
            }
            return new PointCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
