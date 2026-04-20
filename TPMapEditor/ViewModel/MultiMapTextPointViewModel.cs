using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiMapTextPointViewModel : MultiMovableMapObjectViewModel<MapTextPoint>
    {

        private MapTextPoint? selectedMapTextPoint;

        public string Name
        {
            get => selectedMapTextPoint?.Name ?? string.Empty;
            set
            {
                if (selectedMapTextPoint != null) selectedMapTextPoint.Name = value;
            }
        }

        public bool ShowName { get => Count == 1; }

        public string RealText
        {
            get => selectedMapTextPoint?.RealText ?? string.Empty;
            set
            {
                foreach (var item in selectedMapObjects)
                {
                    item.RealText = value;
                }
            }
        }

        public bool Visible
        {
            get => selectedMapTextPoint?.Visible ?? false;
            set
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Visible = value;
                }
            }
        }

        public MultiMapTextPointViewModel(IEnumerable<MapTextPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(MapTextPoint mapObject)
        {
            selectedMapTextPoint = mapObject;
            base.UpdateFromMapObject_Internal(mapObject);
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
            OnPropertyChanged(nameof(RealText));
            OnPropertyChanged(nameof(Visible));
        }
    }
}
