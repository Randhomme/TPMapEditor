using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class Point3 : Point2
    {
        [ObservableProperty]
        private double z;

        partial void OnZChanged(double value)
        {
            OnPropertyChanged();
        }

        public Point3(double x, double y, double z) : base(x, y)
        {
            Z = z;
        }

        public override string ToString()
        {
            return $"{X:0.000000}, {Y:0.000000}, {Z:0.000000}";
        }
    }
}
