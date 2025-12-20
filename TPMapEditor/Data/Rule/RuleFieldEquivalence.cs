using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldEquivalence : RuleField<Equivalence>
    {
        public RuleFieldEquivalence(WorldMap map, string? realLabel, string? label, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
