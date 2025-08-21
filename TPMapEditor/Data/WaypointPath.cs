using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WaypointPath : NamedElement
    {
        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public List<Point3> Points { get; set; }
        public WaypointPath(string name, WorldMap map) : base(map, name)
        {
            Points = new List<Point3>();
            Color = Colors.Black;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.WaypointPaths)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
