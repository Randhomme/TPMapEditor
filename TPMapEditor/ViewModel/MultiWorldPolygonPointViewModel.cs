using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldPolygonPointViewModel : MultiMovableMapObjectViewModel<WorldPolygonPoint>
    {
        public MultiWorldPolygonPointViewModel(IEnumerable<WorldPolygonPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
