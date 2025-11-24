namespace TPMapEditor.Data.Rule
{
    public class RuleFieldUnit : RuleField<ShipUnit>
    {
        public RuleFieldUnit(string? realLabel, string? label, ShipUnit value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
