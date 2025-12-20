namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldFlag : RuleField<Flag>
    {
        public RuleFieldFlag(WorldMap map, string? realLabel, string? label, Flag value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
