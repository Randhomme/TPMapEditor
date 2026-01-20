namespace TPMapEditor.Data.Rule
{
    public class RuleFieldNebulaName : RuleField<Nebula>
    {
        public RuleFieldNebulaName(WorldMap map, string? realLabel, string? label, Nebula? value, bool isOptional, string? optionalLabel, bool isShown) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
