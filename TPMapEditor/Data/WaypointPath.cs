using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WaypointPath : NamedElement
    {
        public static string[] DefaultName => new string[] { "NO PATH", "PATH NAME" };

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        [ObservableProperty]
        private bool isSelected, isLastSelected;
        public ObservableCollection<WaypointPathPoint> Points { get; set; }
        public WaypointPath(WorldMap map, string name) : base(map, name)
        {
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
