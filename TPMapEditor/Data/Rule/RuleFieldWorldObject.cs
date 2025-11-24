namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldObject : RuleField<WorldObject>
    {
        public RuleFieldWorldObject(string? realLabel, string? label, WorldObject value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
