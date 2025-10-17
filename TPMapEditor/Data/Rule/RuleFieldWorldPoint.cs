namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPoint : RuleField<WorldPoint>
    {
        public RuleFieldWorldPoint(string? label, WorldPoint value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
