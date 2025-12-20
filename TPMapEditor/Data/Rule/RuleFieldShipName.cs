namespace TPMapEditor.Data.Rule
{
    public class RuleFieldShipName : RuleField<string>
    {
        public RuleFieldShipName(WorldMap map, string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
