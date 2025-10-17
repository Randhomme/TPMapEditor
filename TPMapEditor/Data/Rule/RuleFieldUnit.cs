namespace TPMapEditor.Data.Rule
{
    public class RuleFieldUnit : RuleField<ShipUnit>
    {
        public RuleFieldUnit(string? label, ShipUnit value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
