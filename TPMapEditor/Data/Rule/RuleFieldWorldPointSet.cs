namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPointSet : RuleField<WorldPointSet>
    {
        public RuleFieldWorldPointSet(string? label, WorldPointSet value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
