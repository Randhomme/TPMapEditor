namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPointSet : RuleField<WorldPointSet>
    {
        public RuleFieldWorldPointSet(WorldMap map, string? realLabel, string? label, WorldPointSet value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value?.ToString() ?? WorldPointSet.DefaultName}'";
        }
    }
}
