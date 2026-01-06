using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class WorldPolygon : MultiPointNamedMapObject<WorldPolygonPoint>
    {
        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public Func<WorldPolygonPoint> WorldPolygonPointFactory { get; }
        public WorldPolygon(WorldMap map, string name) : base(map, name)
        {
            WorldPolygonPointFactory = () => new(this, 0, 0);
            this.color = Colors.Black;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WorldPolygons)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
