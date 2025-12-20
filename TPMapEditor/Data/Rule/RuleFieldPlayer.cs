namespace TPMapEditor.Data.Rule
{
    public class RuleFieldPlayer : RuleField<Player>
    {
        public RuleFieldPlayer(WorldMap map, string? realLabel, string? label, Player value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value?.ToString() ?? Player.DefaultName}'";
        }
    }
}
