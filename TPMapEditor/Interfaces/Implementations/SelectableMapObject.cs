using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Data;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract partial class SelectableMapObject : MapObject, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private int zIndex = 0;

        protected SelectableMapObject(WorldMap map) : base(map)
        {
        }

        public virtual ISelectableMapObject Copy()
        {
            return (ISelectableMapObject)this.MemberwiseClone();
        }
    }
}
