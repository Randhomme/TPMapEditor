using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class WorldPointSet : MultiPointNamedMapObject<WorldPoint>
    {
        public static string DefaultName => "POINT SET";

        public static WorldPointSet DefaultWorldPointSet { get; } = new(null, DefaultName);

        [ObservableProperty]
        private Color color = Colors.Black;

        public Func<WorldPoint> WorldPointFactory { get; }

        public WorldPointSet(WorldMap map, string name) : base(map, name)
        {
            WorldPointFactory = () => new(this, 0, 0, 0, 0);
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

        public override ISelectableMapObject Copy()
        {
            var copy = new WorldPointSet(Map, GenerateName($"{Name}_", Map.WorldPointSets))
            {
                Color = this.Color
            };
            for (int i = 0; i < Points.Count; i++)
            {
                var p = (WorldPoint)Points[i].Copy();
                p.IsSelected = p.IsLastSelected = false;
                p.Parent = copy;
                copy.Points.Add(p);
            }
            return copy;
        }
    }
}
