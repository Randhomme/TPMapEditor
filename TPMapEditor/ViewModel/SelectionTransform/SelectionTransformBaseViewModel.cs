using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel.SelectionTransform
{
    public abstract partial class SelectionTransformBaseViewModel : ObservableObject
    {
        private readonly IUndoManagerService undoManagerService;
        protected abstract IUndoableMapCommand Command { get; }

        public bool ShouldCommitCommand { get; set; } = false;
        public bool Is3D { get; set; } = false;

        protected SelectionTransformBaseViewModel(IUndoManagerService undoManagerService)
        {
            this.undoManagerService = undoManagerService;
        }

        public void CommitCommand()
        {
            Command.Commit();
            undoManagerService.Push(Command);
        }

        public void CancelCommand()
        {
            Command.Undo();
        }
    }
}
