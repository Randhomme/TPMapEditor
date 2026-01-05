using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
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
    }
}
