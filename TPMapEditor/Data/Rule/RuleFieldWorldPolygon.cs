namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldPolygon : RuleField<WorldPolygon>
    {
        public RuleFieldWorldPolygon(string? realLabel, string? label, WorldPolygon value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
