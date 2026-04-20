using System.Collections.Generic;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPolygonViewModel : MultiSelectableMapObjectViewModel<WorldPolygon>, IColoredMapObject
    {
        private WorldPolygon? selectedWorldPolygon;

        public string Name
        {
            get => selectedWorldPolygon?.Name ?? string.Empty;
            set
            {
                if (selectedWorldPolygon != null) selectedWorldPolygon.Name = value;
            }
        }
        public bool ShowName { get => Count == 1; }

        public Color Color
        {
            get => selectedWorldPolygon?.Color ?? Colors.Black;
            set
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Color = value;
                }
            }
        }

        public MultiWorldPolygonViewModel(IEnumerable<WorldPolygon> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WorldPolygon mapObject)
        {
            selectedWorldPolygon = mapObject;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
            OnPropertyChanged(nameof(Color));
        }
    }
}
