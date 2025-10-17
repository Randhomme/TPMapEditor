namespace TPMapEditor.Data.Rule
{
    public class RuleFieldPlayer : RuleField<Player>
    {
        public RuleFieldPlayer(string? label, Player value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
