namespace TPMapEditor.Data.Rule
{
    public class RuleFieldInGameMessage : RuleField<string>
    {
        public RuleFieldInGameMessage(string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
