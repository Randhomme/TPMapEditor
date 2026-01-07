using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
{
    public partial class WorldPoint : Point3, ISelectableMapObject, IMovableMapObject
    {
        [ObservableProperty]
        private double xRotation, yRotation, zRotation, magnitude;
        [ObservableProperty]
        private WorldPointSet parent;
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private int zIndex = 0;

        public WorldPoint(WorldPointSet parent, double x, double y, double z, double zRotation) : base(x, y, z)
        {
            this.xRotation = this.yRotation = this.magnitude = 0;
            this.zRotation = zRotation;
            this.parent = parent;
        }

        public ISelectableMapObject Copy()
        {
            return (ISelectableMapObject)this.MemberwiseClone();
        }
    }
}
