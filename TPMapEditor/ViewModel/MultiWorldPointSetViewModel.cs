using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPointSetViewModel : MultiSelectableMapObjectViewModel<WorldPointSet>
    {
        public MultiWorldPointSetViewModel(IEnumerable<WorldPointSet> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(WorldPointSet mapObject)
        {
            throw new System.NotImplementedException();
        }
    }
}
