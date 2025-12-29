using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WaypointPath : SelectableNamedMapObject
    {
        public static string[] DefaultName => new string[] { "NO PATH", "PATH NAME" };

        public static WaypointPath DefaultWaypointPath { get; } = new(null, DefaultName[0]);

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public ObservableCollection<WaypointPathPoint> Points { get; set; }
        public Func<WaypointPathPoint> WaypointPathPointFactory { get; }
        public WaypointPath(WorldMap map, string name) : base(map, name)
        {
            WaypointPathPointFactory = () => new(this, 0, 0, 0);
            Points = new ObservableCollection<WaypointPathPoint>();
            Points.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Points));
            };
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
    }
}
