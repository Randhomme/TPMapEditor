namespace TPMapEditor.Data.Rule
{
    public class RuleFieldPlayer : RuleField<Player>
    {
        public RuleFieldPlayer(string? realLabel, string? label, Player value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
