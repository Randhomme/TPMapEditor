using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Services
{
    public interface ISelectionKeyboardShortcutService
    {
        /// <summary>
        /// Hides selected objects
        /// </summary>
        public void OnHKey();

        /// <summary>
        /// Shows hidden objects
        /// </summary>
        public void OnShiftHKey();

        /// <summary>
        /// Toggles selected objets visibility
        /// </summary>
        public void OnCtrlHKey();

        /// <summary>
        /// Selects all objects
        /// </summary>
        public void OnAKey();

        /// <summary>
        /// Unselects all objects
        /// </summary>
        public void OnShiftAKey();

        /// <summary>
        /// Inverts selection.
        /// </summary>
        public void OnCtrlAKey();
    }
}
