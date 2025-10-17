namespace TPMapEditor.Data.Rule
{
    public class RuleFieldShipName : RuleField<string>
    {
        public RuleFieldShipName(string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
