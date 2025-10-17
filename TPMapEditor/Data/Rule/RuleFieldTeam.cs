namespace TPMapEditor.Data.Rule
{
    public class RuleFieldTeam : RuleField<Team>
    {
        public RuleFieldTeam(string? label, Team value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
