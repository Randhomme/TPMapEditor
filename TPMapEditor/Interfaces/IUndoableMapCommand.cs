using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Represents a map command that can be undone/redone.
    /// </summary>
    public interface IUndoableMapCommand
    {
        /// <summary>
        /// Apply the command
        /// </summary>
        public void Apply();

        /// <summary>
        /// Undo the command
        /// </summary>
        public void Undo();

        /// <summary>
        /// Redo the command
        /// </summary>
        public void Redo();
    }
}
