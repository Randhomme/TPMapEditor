using System;
using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWaypointPathViewModel : MultiSelectableMapObjectViewModel<WaypointPath>
    {
        public MultiWaypointPathViewModel(IEnumerable<WaypointPath> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WaypointPath mapObject)
        {
            throw new NotImplementedException();
        }
    }
}
