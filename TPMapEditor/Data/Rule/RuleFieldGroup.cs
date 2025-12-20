namespace TPMapEditor.Data.Rule
{
    public class RuleFieldGroup : RuleField<Group>
    {
        public RuleFieldGroup(WorldMap map, string? realLabel, string? label, Group value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
