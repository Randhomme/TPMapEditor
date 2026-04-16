using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiObjectivePointViewModel : MultiMovableMapObjectViewModel<ObjectivePoint>
    {
        public MultiObjectivePointViewModel(IEnumerable<ObjectivePoint> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
