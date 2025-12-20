namespace TPMapEditor.Data.Rule
{
    public class RuleFieldInGameMessage : RuleField<string>
    {
        public RuleFieldInGameMessage(WorldMap map, string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
