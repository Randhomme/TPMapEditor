using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldGroupUnit : RuleField<NamedMapObject>
    {
        [ObservableProperty]
        private bool isGroupUnitUnit = false; //true if Unit, false if Group
        [ObservableProperty]
        private Group selectedGroup;

        public RuleFieldGroupUnit(WorldMap map, string? realLabel, string? label, Group selectedGroup, NamedMapObject value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.selectedGroup = selectedGroup;
        }

        public override string ToString()
        {
            if (IsGroupUnitUnit)
                return $"{RealLabel} '{SelectedGroup.Name},{Value?.Name ?? ShipUnit.DefaultName}'";
            return $"{RealLabel} '{Value}'";
        }
    }
}
