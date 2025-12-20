namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWaypointPath : RuleField<WaypointPath>
    {
        public RuleFieldWaypointPath(WorldMap map, string? realLabel, string? label, WaypointPath value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value?.ToString() ?? WaypointPath.DefaultName[0]}'";
        }
    }
}
