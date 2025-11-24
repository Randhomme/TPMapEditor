using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldEquivalence : RuleField<Equivalence>
    {
        public RuleFieldEquivalence(string? realLabel, string? label, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
