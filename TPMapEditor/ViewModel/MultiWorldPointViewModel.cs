using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPointViewModel : MultiRotatableMapObjectViewModel<WorldPoint>
    {
        public MultiWorldPointViewModel(IEnumerable<WorldPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
