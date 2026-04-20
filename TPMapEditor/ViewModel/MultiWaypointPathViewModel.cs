using System.Collections.Generic;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWaypointPathViewModel : MultiSelectableMapObjectViewModel<WaypointPath>, IColoredMapObject
    {
        private WaypointPath? selectedWaypointPath;

        public string Name
        {
            get => selectedWaypointPath?.Name ?? string.Empty;
            set
            {
                if (selectedWaypointPath != null) selectedWaypointPath.Name = value;
            }
        }
        public bool ShowName { get => Count == 1; }

        public Color Color
        {
            get => selectedWaypointPath?.Color ?? Colors.Black;
            set
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Color = value;
                }
            }
        }

        public MultiWaypointPathViewModel(IEnumerable<WaypointPath> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WaypointPath mapObject)
        {
            selectedWaypointPath = mapObject;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
            OnPropertyChanged(nameof(Color));
        }
    }
}
