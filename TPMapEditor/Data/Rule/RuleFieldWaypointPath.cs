namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWaypointPath : RuleField<WaypointPath>
    {
        public RuleFieldWaypointPath(string? label, WaypointPath value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
