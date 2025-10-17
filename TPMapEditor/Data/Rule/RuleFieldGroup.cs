namespace TPMapEditor.Data.Rule
{
    public class RuleFieldGroup : RuleField<Group>
    {
        public RuleFieldGroup(string? label, Group value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
