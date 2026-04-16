using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldObjectViewModel : MultiRotatableMapObjectViewModel<WorldObject>
    {
        [ObservableProperty]
        WorldObjectType type;

        public MultiWorldObjectViewModel(IEnumerable<WorldObject> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
            type = WorldObjectType.WotTypes.FirstOrDefault();
        }

        protected override void UpdateFromMapObject_Internal(WorldObject mapObject)
        {
            base.UpdateFromMapObject_Internal(mapObject);
            Type = mapObject.Type;
        }

        partial void OnTypeChanged(WorldObjectType? value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Type = Type;
                }
            }
        }
    }
}
