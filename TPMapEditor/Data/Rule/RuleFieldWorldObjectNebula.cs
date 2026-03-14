namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldObjectNebula : RuleFieldWorldObject
    {
        public RuleFieldWorldObjectNebula(WorldMap map, string? realLabel, string? label, WorldObject value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
