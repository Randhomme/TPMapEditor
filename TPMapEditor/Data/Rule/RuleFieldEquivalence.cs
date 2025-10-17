using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldEquivalence : RuleField<Equivalence>
    {
        public RuleFieldEquivalence(string? label = null, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
