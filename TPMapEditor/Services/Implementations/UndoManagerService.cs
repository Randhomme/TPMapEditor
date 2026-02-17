using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class UndoManagerService : ObservableObject, IUndoManagerService
    {
        private readonly int _max;
        private readonly Stack<IUndoableMapCommand> _undo = new();
        private readonly Stack<IUndoableMapCommand> _redo = new();

        public bool CanUndo { get => _undo.Count > 0; }
        public bool CanRedo { get => _redo.Count > 0; }

        public UndoManagerService(int max = 10)
        {
            _max = max;
        }

        public void Push(IUndoableMapCommand cmd)
        {
            _undo.Push(cmd);
            _redo.Clear();

            if (_undo.Count > _max)
                _undo.Pop();

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public void Undo()
        {
            if (_undo.Count == 0) return;
            var cmd = _undo.Pop();
            cmd.Undo();
            _redo.Push(cmd);
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public void Redo()
        {
            if (_redo.Count == 0) return;
            var cmd = _redo.Pop();
            cmd.Redo();
            _undo.Push(cmd);
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
    }
}
