namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPolygon : RuleField<WorldPolygon>
    {
        public RuleFieldWorldPolygon(WorldMap map, string? realLabel, string? label, WorldPolygon value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
