using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
{
    public partial class WorldPolygonPoint : Point2, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private WorldPolygon parent;
        [ObservableProperty]
        private int zIndex = 0;

        public WorldPolygonPoint(WorldPolygon parent, double x, double y) : base(x, y)
        {
            this.parent = parent;
        }
    }
}
