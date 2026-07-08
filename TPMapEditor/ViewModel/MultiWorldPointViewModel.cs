using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPointViewModel : MultiRotatableMapObjectViewModel<WorldPoint>
    {
        [ObservableProperty]
        private double magnitude;

        public MultiWorldPointViewModel(IEnumerable<WorldPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WorldPoint mapObject)
        {
            base.UpdateFromMapObject_Internal(mapObject);
            Magnitude = mapObject.Magnitude;
        }

        partial void OnMagnitudeChanged(double value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Magnitude = Magnitude;
                }
            }
        }
    }
}
