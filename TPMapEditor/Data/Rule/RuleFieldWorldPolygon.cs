namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPolygon : RuleField<WorldPolygon>
    {
        public RuleFieldWorldPolygon(string? label, WorldPolygon value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
