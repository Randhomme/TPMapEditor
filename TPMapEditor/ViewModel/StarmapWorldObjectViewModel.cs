using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using TPMapEditor.Data;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.ViewModel
{
    public partial class StarmapWorldObjectViewModel : ObservableObject
    {
        [ObservableProperty]
        private BitmapSource starmapImage;

        public double X { get; }
        public double Y { get; }
        public double ZRotation { get; }
        public double ZIndex { get; }
        public Point Pivot { get; }
        public BitmapSource OriginalImage { get; }
        public CustomInfoDefinition CustomInfo { get; }
        public StarmapWorldObjectViewModel(WorldObject worldObject)
        {
            X = worldObject.X;
            Y = worldObject.Y;
            ZRotation = worldObject.ZRotation;
            ZIndex = worldObject.ZIndex;
            Pivot = worldObject.Type.Pivot;
            OriginalImage = starmapImage = worldObject.Type.Image;
            CustomInfo = worldObject.Type.CustomInfoDefinition;
        }
    }
}
