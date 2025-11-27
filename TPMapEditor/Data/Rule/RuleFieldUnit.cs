using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldUnit : RuleField<ShipUnit>
    {
        [ObservableProperty]
        private Group selectedGroup;

        public IEnumerable<ShipUnit> AvailableShipUnits => SelectedGroup.ShipUnits;

        partial void OnSelectedGroupChanged(Group value)
        {
            OnPropertyChanged(nameof(AvailableShipUnits));
            Value = AvailableShipUnits.FirstOrDefault();
        }

        public RuleFieldUnit(string? realLabel, string? label, Group selectedGroup, ShipUnit value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.selectedGroup = selectedGroup;
        }

        public override string ToString()
        {
            return $"{SelectedGroup.Name},{Value?.Name ?? "HUMAN CONTROLLED SHIP"}";
        }
    }
}
