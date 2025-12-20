namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldFlagTexture : RuleField<string>
    {
        public RuleFieldFlagTexture(WorldMap map, string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
