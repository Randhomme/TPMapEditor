namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPointSet : RuleField<WorldPointSet>
    {
        public RuleFieldWorldPointSet(string? realLabel, string? label, WorldPointSet value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value?.ToString() ?? WorldPointSet.DefaultName}'";
        }
    }
}
