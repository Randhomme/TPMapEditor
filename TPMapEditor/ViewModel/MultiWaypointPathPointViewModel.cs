using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWaypointPathPointViewModel : MultiMovableMapObjectViewModel<WaypointPathPoint>
    {
        public MultiWaypointPathPointViewModel(IEnumerable<WaypointPathPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
