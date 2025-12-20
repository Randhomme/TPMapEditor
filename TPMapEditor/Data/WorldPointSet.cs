using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WorldPointSet : NamedElement
    {
        public static string DefaultName => "POINT SET";

        [ObservableProperty]
        private Color color = Colors.Black;
        [ObservableProperty]
        private bool isSelected, isLastSelected;

        public ObservableCollection<WorldPoint> Points { get; }

        public Func<WorldPoint> WorldPointFactory { get; }

        public WorldPointSet(WorldMap map, string name) : base(map, name)
        {
            WorldPointFactory = () => new(this, 0, 0, 0, 0);
            Points = new ObservableCollection<WorldPoint>();
            Points.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Points));
            };
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WorldPointSets)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }
    }
}
