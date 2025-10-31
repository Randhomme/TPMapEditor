using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class WaypointPathPoint : ObservableObject
    {
        [ObservableProperty]
        private double x, y, z;
        [ObservableProperty]
        private bool isSelected;
        [ObservableProperty]
        private WaypointPath parent;

        public WaypointPathPoint(WaypointPath parent, double x, double y, double z)
        {
            this.parent = parent;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
