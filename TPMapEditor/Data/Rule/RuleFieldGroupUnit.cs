using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldGroupUnit : RuleField<NamedElement>
    {
        [ObservableProperty]
        private bool isGroupUnitUnit = false; //true if Unit, false if Group
        [ObservableProperty]
        private Group selectedGroup; //for unit selection

        public IEnumerable<ShipUnit> AvailableShipUnits => SelectedGroup.ShipUnits;

        partial void OnSelectedGroupChanged(Group value)
        {
            OnPropertyChanged(nameof(AvailableShipUnits));
            if (IsGroupUnitUnit)
                Value = AvailableShipUnits.FirstOrDefault();
        }

        public RuleFieldGroupUnit(string? realLabel, string? label, Group selectedGroup, NamedElement value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.selectedGroup = selectedGroup;
        }

        public override string ToString()
        {
            if (IsGroupUnitUnit)
                return $"{SelectedGroup.Name},{Value?.Name ?? "HUMAN CONTROLLED SHIP"}";
            return Value?.Name ?? "Player0 Group";
        }
    }
}
