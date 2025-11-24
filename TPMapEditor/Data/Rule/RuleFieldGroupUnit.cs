using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldGroupUnit : RuleField<NamedElement>
    {
        [ObservableProperty]
        private bool isGroupUnitUnit = false; //true if Unit, false if Group

        public RuleFieldGroupUnit(string? realLabel, string? label, NamedElement value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
