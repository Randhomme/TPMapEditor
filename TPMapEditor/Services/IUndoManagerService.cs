using System.ComponentModel;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services
{
    public interface IUndoManagerService : INotifyPropertyChanged
    {
        public bool CanUndo { get; }
        public bool CanRedo { get; }

        /// <summary>
        /// Adds an action that can be undone
        /// </summary>
        /// <param name="action"></param>
        public void Push(IUndoableMapCommand action);

        /// <summary>
        /// Undo the last action
        /// </summary>
        public void Undo();

        /// <summary>
        /// Redo the last action
        /// </summary>
        public void Redo();

        /// <summary>
        /// Clears the undo and redo commands
        /// </summary>
        public void Clear();
    }
}
