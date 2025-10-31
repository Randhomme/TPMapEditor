using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace TPMapEditor.Data
{
    public partial class Point3 : Point2
    {
        [ObservableProperty]
        private double z;

        public Point3(double x, double y, double z) : base(x, y)
        {
            Z = z;
        }
    }
}
