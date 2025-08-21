using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WorldPoint : NamedElement
    {
        [ObservableProperty]
        private double x, y, z, xRotation, yRotation, zRotation, magnitude; // still have to figure out the purpose of magnitude
        [ObservableProperty]
        private Color color;

        public WorldPoint(string name, WorldMap map, double x, double y, double zRotation) : base(map, name)
        {
            this.x = x;
            this.y = y;
            this.z = this.xRotation = this.yRotation = this.magnitude = 0;
            this.zRotation = zRotation;
            this.color = Colors.Black;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.WorldPoints)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
