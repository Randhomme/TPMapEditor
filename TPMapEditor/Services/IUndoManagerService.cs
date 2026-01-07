using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
