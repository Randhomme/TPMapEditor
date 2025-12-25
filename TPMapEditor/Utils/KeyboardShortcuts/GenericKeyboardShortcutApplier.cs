using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Data;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Utils.KeyboardShortcuts
{
    /// <summary>
    /// Provides an abstract base for applying keyboard shortcuts on the map.
    /// </summary>
    /// <remarks>Implement this class for a type of map object displayed on the UI.</remarks>
    public abstract class KeyboardShortcutApplier
    {
        /// <summary>
        /// Handles the event triggered when the H key is pressed.
        /// </summary>
        public abstract void OnHKey();

        /// <summary>
        /// Handles the event when the Shift+H key combination is pressed.
        /// </summary>
        public abstract void OnShiftHKey();

        /// <summary>
        /// Handles the event when the Ctrl+H key combination is pressed.
        /// </summary>
        public abstract void OnCtrlHKey();

        /// <summary>
        /// Handles the event triggered when the A key is pressed.
        /// </summary>
        public abstract void OnAKey();

        /// <summary>
        /// Handles the event when the Shift+A key combination is pressed.
        /// </summary>
        public abstract void OnShiftAKey();

        /// <summary>
        /// Handles the event when the Ctrl+A key combination is pressed.
        /// </summary>
        public abstract void OnCtrlAKey();
    }
}
