namespace TPMapEditor.Data.Rule
{
    public class RuleFieldGuiTexture : RuleField<string>
    {
        public RuleFieldGuiTexture(WorldMap map, string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
