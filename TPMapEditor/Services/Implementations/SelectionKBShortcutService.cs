using System;
using System.Collections.Generic;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class SelectionKBShortcutService<T> : ISelectionKBShortcutService where T : ISelectableMapObject
    {
        private IEnumerable<T> mapObjects;
        private ISelectionService<T> selectionService;

        public SelectionKBShortcutService(IEnumerable<T> mapObjects, ISelectionService<T> selectionService)
        {
            this.mapObjects = mapObjects;
            this.selectionService = selectionService;
        }

        public void OnHKey()
        {
            selectionService.HideSelection();
        }

        public void OnShiftHKey()
        {
            selectionService.ShowAll(mapObjects);
        }

        public void OnCtrlHKey()
        {
            selectionService.ToggleSelectionVisibility();
        }

        public void OnAKey()
        {
            selectionService.AddAllToSelection(mapObjects);
        }

        public void OnShiftAKey()
        {
            selectionService.ClearSelection();
        }

        public void OnCtrlAKey()
        {
            selectionService.InvertSelection(mapObjects);
        }
    }
}
