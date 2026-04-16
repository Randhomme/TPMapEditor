using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public abstract class MultiSelectableMapObjectViewModel<T> : ObservableObject, IMultiSelectableMapObject where T : ISelectableMapObject
    {
        protected readonly IUndoManagerService undoManagerService;
        protected readonly IEnumerable<T> selectedMapObjects;

        public bool UseUpdateCommands { get; set; } = true;

        public int Count { get => selectedMapObjects.Count(); }

        public MultiSelectableMapObjectViewModel(IEnumerable<T> selectedMapObjects, IUndoManagerService undoManagerService)
        {
            this.selectedMapObjects = selectedMapObjects;
            this.undoManagerService = undoManagerService;
        }

        public void UpdateFromMapObject(T? mapObject)
        {
            if (mapObject != null)
            {
                UseUpdateCommands = false;
                UpdateFromMapObject_Internal(mapObject);
                UseUpdateCommands = true;
            }
        }

        protected abstract void UpdateFromMapObject_Internal(T mapObject);
    }
}
