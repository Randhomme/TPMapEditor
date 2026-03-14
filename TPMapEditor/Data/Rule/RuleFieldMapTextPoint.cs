namespace TPMapEditor.Data.Rule
{
    public class RuleFieldMapTextPoint : RuleField<MapTextPoint>
    {
        public RuleFieldMapTextPoint(WorldMap map, string? realLabel, string? label, MapTextPoint value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
