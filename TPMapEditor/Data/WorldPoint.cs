using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class WorldPoint : Point3
    {
        [ObservableProperty]
        private double xRotation, yRotation, zRotation, magnitude; // still have to figure out the purpose of magnitude
        [ObservableProperty]
        private WorldPointSet parent;

        public WorldPoint(WorldPointSet parent, double x, double y, double z, double zRotation) : base(x, y, z)
        {
            this.xRotation = this.yRotation = this.magnitude = 0;
            this.zRotation = zRotation;
            this.parent = parent;
        }
    }
}
