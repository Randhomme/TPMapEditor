using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Data;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract partial class SelectableNamedMapObject : NamedMapObject, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
        [ObservableProperty]
        private int zIndex = 0;

        protected SelectableNamedMapObject(WorldMap map, string name) : base(map, name)
        {
        }

        public virtual ISelectableMapObject Copy()
        {
            return (ISelectableMapObject)this.MemberwiseClone();
        }
    }
}
