namespace TPMapEditor.Services
{
    public interface ISelectionKBShortcutService
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

        /// <summary>
        /// Makes a copy of the selection
        /// </summary>
        public void OnCtrlC();

        /// <summary>
        /// Pastes the selection onto the map
        /// </summary>
        public void OnCtrlV();
    }
}
