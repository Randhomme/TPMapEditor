using System.Collections.Generic;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiPlayerViewModel : MultiRotatableMapObjectViewModel<Player>
    {
        public MultiPlayerViewModel(IEnumerable<Player> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }
    }
}
