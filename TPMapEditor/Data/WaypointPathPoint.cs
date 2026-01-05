using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
{
    public partial class WaypointPathPoint : Point3, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private int zIndex = 0;
        [ObservableProperty]
        private WaypointPath parent;

        public WaypointPathPoint(WaypointPath parent, double x, double y, double z) : base(x, y, z)
        {
            this.parent = parent;
        }
    }
}
