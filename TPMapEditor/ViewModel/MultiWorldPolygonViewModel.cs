using System;
using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPolygonViewModel : MultiSelectableMapObjectViewModel<WorldPolygon>
    {
        public MultiWorldPolygonViewModel(IEnumerable<WorldPolygon> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WorldPolygon mapObject)
        {
            throw new NotImplementedException();
        }
    }
}
