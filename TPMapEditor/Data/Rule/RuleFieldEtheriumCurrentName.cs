namespace TPMapEditor.Data.Rule
{
    internal class RuleFieldEtheriumCurrentName : RuleField<EtheriumCurrent>
    {
        public RuleFieldEtheriumCurrentName(WorldMap map, string? realLabel, string? label, EtheriumCurrent? value, bool isOptional, string? optionalLabel, bool isShown) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
