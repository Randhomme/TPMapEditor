using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiObjectivePointViewModel : MultiMovableMapObjectViewModel<ObjectivePoint>
    {
        private ObjectivePoint? selectedObjectivePoint;

        public string Name
        {
            get => selectedObjectivePoint?.Name ?? string.Empty;
            set
            {
                if (selectedObjectivePoint != null) selectedObjectivePoint.Name = value;
            }
        }

        public bool ShowName { get => Count == 1; }

        public MultiObjectivePointViewModel(IEnumerable<ObjectivePoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(ObjectivePoint mapObject)
        {
            selectedObjectivePoint = mapObject;
            base.UpdateFromMapObject_Internal(mapObject);
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
        }
    }
}
