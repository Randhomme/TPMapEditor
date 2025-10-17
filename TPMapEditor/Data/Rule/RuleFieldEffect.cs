namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldEffect : RuleField<string>
    {
        public RuleFieldEffect(string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
