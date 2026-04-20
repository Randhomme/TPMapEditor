using System.Collections.Generic;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPointSetViewModel : MultiSelectableMapObjectViewModel<WorldPointSet>, IColoredMapObject
    {
        private WorldPointSet? selectedWorldPointSet;

        public string Name
        {
            get => selectedWorldPointSet?.Name ?? string.Empty;
            set
            {
                if (selectedWorldPointSet != null) selectedWorldPointSet.Name = value;
            }
        }
        public bool ShowName { get => Count == 1; }

        public Color Color
        {
            get => selectedWorldPointSet?.Color ?? Colors.Black;
            set
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Color = value;
                }
            }
        }

        public MultiWorldPointSetViewModel(IEnumerable<WorldPointSet> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WorldPointSet mapObject)
        {
            selectedWorldPointSet = mapObject;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
            OnPropertyChanged(nameof(Color));
        }
    }
}
