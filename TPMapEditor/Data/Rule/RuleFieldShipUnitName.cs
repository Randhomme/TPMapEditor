namespace TPMapEditor.Data.Rule
{
    public class RuleFieldShipUnitName : RuleField<ShipUnit>
    {
        public RuleFieldShipUnitName(WorldMap map, string? realLabel, string? label, ShipUnit unit, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, unit, isOptional, optionalLabel, isShown)
        {
        }
    }
}
