using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
{
    public partial class WorldPoint : Point3, ISelectableMapObject, IMovableMapObject, IRotatableMapObject
    {
        [ObservableProperty]
        private double xRotation = 90, yRotation, displayedZRotation, magnitude;
        [ObservableProperty]
        private WorldPointSet parent;
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private int zIndex = 0;

        private double zRotation;
        public double ZRotation
        {
            get => zRotation;
            set
            {
                SetProperty(ref zRotation, value);
                DisplayedZRotation = -value;
            }
        }

        public WorldPoint(WorldPointSet parent, double x, double y, double z, double zRotation) : base(x, y, z)
        {
            this.xRotation = 90;
            this.yRotation = this.magnitude = 0;
            this.ZRotation = zRotation;
            this.parent = parent;
        }

        public ICopiableMapObject Copy()
        {
            return (ISelectableMapObject)this.MemberwiseClone();
        }
    }
}
