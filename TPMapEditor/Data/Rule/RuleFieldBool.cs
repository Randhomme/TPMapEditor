namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldBool : RuleField<bool>
    {
        public RuleFieldBool(string? label = null, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
