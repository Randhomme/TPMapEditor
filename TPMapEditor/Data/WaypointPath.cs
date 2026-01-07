using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class WaypointPath : MultiPointNamedMapObject<WaypointPathPoint>
    {
        public static string[] DefaultName => new string[] { "NO PATH", "PATH NAME" };

        public static WaypointPath DefaultWaypointPath { get; } = new(null, DefaultName[0]);

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public Func<WaypointPathPoint> WaypointPathPointFactory { get; }

        public WaypointPath(WorldMap map, string name) : base(map, name)
        {
            WaypointPathPointFactory = () => new(this, 0, 0, 0);
            Color = Colors.Black;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WaypointPaths)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return DefaultName.Contains(name);
        }

        public override ISelectableMapObject Copy()
        {
            var copy = new WaypointPath(Map, GenerateName($"{Name}_", Map.WaypointPaths))
            {
                Color = this.Color
            };
            for (int i = 0; i < Points.Count; i++)
            {
                var p = (WaypointPathPoint)Points[i].Copy();
                p.IsSelected = p.IsLastSelected = false;
                p.Parent = copy;
                copy.Points.Add(p);
            }
            return copy;
        }
    }
}
