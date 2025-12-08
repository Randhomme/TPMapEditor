using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldUnit : RuleField<ShipUnit>
    {
        [ObservableProperty]
        private Group selectedGroup;

        public RuleFieldUnit(string? realLabel, string? label, Group selectedGroup, ShipUnit value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.selectedGroup = selectedGroup;
        }

        public override string ToString()
        {
            return $"{RealLabel} '{SelectedGroup.Name},{Value?.Name ?? ShipUnit.DefaultName}'";
        }
    }
}
