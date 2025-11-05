using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class WorldPolygonPoint : Point2
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected;
        [ObservableProperty]
        private WorldPolygon parent;

        public WorldPolygonPoint(WorldPolygon parent, double x, double y) : base(x, y)
        {
            this.parent = parent;
        }
    }
}
