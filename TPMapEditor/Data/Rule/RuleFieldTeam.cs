namespace TPMapEditor.Data.Rule
{
    public class RuleFieldTeam : RuleField<Team>
    {
        public RuleFieldTeam(string? realLabel, string? label, Team value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
