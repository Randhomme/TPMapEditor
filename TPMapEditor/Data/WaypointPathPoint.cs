using CommunityToolkit.Mvvm.ComponentModel;
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
