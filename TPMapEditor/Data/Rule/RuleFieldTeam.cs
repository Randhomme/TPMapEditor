namespace TPMapEditor.Data.Rule
{
    public class RuleFieldTeam : RuleField<Team>
    {
        public RuleFieldTeam(WorldMap map, string? realLabel, string? label, Team value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
