namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldEffect : RuleField<string>
    {
        public RuleFieldEffect(string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
