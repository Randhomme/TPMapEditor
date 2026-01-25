using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class Point2EnumerableToPathRoundedGeometry : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Point2> points)
            {
                var pts = points.Select((p) => p.Point).ToList();
                if (pts.Count < 2)
                    return Geometry.Empty;

                var figure = new PathFigure
                {
                    StartPoint = pts[0],
                    IsFilled = true
                };

                // Cas segment simple
                if (pts.Count == 2)
                {
                    figure.Segments.Add(new LineSegment(pts[1], true));
                    return new PathGeometry(new[] { figure });
                }

                // Cas >= 3 points : coins arrondis
                for (int i = 1; i < pts.Count - 1; i++)
                {
                    Point prev = pts[i - 1];
                    Point curr = pts[i];
                    Point next = pts[i + 1];

                    Vector v1 = prev - curr;
                    Vector v2 = next - curr;

                    if (v1.Length == 0 || v2.Length == 0)
                        continue;

                    v1.Normalize();
                    v2.Normalize();

                    double r = 180;
                    r = Math.Min(r, (prev - curr).Length / 2);
                    r = Math.Min(r, (next - curr).Length / 2);

                    Point p1 = curr + v1 * r;
                    Point p2 = curr + v2 * r;

                    figure.Segments.Add(new LineSegment(p1, true));
                    figure.Segments.Add(new QuadraticBezierSegment(curr, p2, true));
                }

                // Dernier segment
                figure.Segments.Add(new LineSegment(pts.Last(), true));

                return new PathGeometry(new[] { figure });
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
