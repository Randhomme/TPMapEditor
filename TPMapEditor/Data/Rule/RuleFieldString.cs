namespace TPMapEditor.Data.Rule
{
    public class RuleFieldString : RuleField<string>
    {
        public RuleFieldString(string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
