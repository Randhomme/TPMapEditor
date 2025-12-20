namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldBool : RuleField<bool>
    {
        public RuleFieldBool(WorldMap map, string? realLabel, string? label, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.ToString().ToUpperInvariant()}'";
        }
    }
}
