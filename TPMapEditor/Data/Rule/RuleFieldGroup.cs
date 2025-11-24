namespace TPMapEditor.Data.Rule
{
    public class RuleFieldGroup : RuleField<Group>
    {
        public RuleFieldGroup(string? realLabel, string? label, Group value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
