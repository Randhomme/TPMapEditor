using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WorldPolygon : NamedElement
    {
        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public List<Point2> Points { get; set; }
        public WorldPolygon(string name, WorldMap map) : base(map, name)
        {
            Points = new List<Point2>();
            this.color = Colors.Black;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.WorldPolygons)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
