namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldBool : RuleField<bool>
    {
        public RuleFieldBool(string? realLabel, string? label, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
