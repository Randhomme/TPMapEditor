namespace TPMapEditor.Data.Rule
{
    public class RuleFieldString : RuleField<string>
    {
        public RuleFieldString(string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
