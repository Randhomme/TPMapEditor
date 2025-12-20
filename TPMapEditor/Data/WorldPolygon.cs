using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class WorldPolygon : NamedElement
    {
        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        [ObservableProperty]
        private bool isSelected, isLastSelected;
        public ObservableCollection<WorldPolygonPoint> Points { get; set; }
        public Func<WorldPolygonPoint> WorldPolygonPointFactory { get; }
        public WorldPolygon(WorldMap map, string name) : base(map, name)
        {
            WorldPolygonPointFactory = () => new(this, 0, 0);
            Points = new ObservableCollection<WorldPolygonPoint>();
            Points.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Points));
            };
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
