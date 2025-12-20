using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace TPMapEditor.Data
{
    public partial class Point2 : ObservableObject
    {
        [ObservableProperty]
        private Point point;

        public double X
        {
            get => Point.X;
            set
            {
                Point = new Point(value, Point.Y);
                OnPropertyChanged();
            }
        }
        public double Y
        {
            get => Point.Y;
            set
            {
                Point = new Point(Point.X, value);
                OnPropertyChanged();
            }
        }

        public Point2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X:0.000000}, {Y:0.000000}";
        }
    }
}
