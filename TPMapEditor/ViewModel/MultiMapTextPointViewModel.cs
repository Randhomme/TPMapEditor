using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiMapTextPointViewModel : MultiMovableMapObjectViewModel<MapTextPoint>
    {
        public MultiMapTextPointViewModel(IEnumerable<MapTextPoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
