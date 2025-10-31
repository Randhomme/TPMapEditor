using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class WaypointPathPoint : Point3
    {
        [ObservableProperty]
        private bool isSelected;
        [ObservableProperty]
        private WaypointPath parent;

        public WaypointPathPoint(WaypointPath parent, double x, double y, double z) : base(x, y, z)
        {
            this.parent = parent;   
        }
    }
}
