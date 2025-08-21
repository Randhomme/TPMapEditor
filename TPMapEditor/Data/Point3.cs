using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace TPMapEditor.Data
{
    public partial class Point3 : ObservableObject
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

        [ObservableProperty]
        private double z;

        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
