using System.Collections.Generic;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class SelectionKBShortcutService<T> : ISelectionKBShortcutService where T : ISelectableMapObject
    {
        private readonly IList<T> mapObjects;
        private readonly ISelectionService<T> selectionService;
        private readonly ICopyPasteService copyPasteService;

        public SelectionKBShortcutService(IList<T> mapObjects, ISelectionService<T> selectionService, ICopyPasteService copyPasteService)
        {
            this.mapObjects = mapObjects;
            this.selectionService = selectionService;
            this.copyPasteService = copyPasteService;
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

        public void OnCtrlC()
        {
            copyPasteService.Copy(selectionService.SelectedMapObjects);
        }

        public void OnCtrlV()
        {
            var pastedItems = copyPasteService.Paste<T>();
            foreach (var item in pastedItems)
            {
                selectionService.Unselect(item);
                mapObjects.Add(item);
            }
            selectionService.ClearSelection();
            selectionService.AddAllToSelection(pastedItems);
        }
    }
}
