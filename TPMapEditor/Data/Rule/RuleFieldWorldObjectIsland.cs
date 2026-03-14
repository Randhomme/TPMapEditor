namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldObjectIsland : RuleFieldWorldObject
    {
        public RuleFieldWorldObjectIsland(WorldMap map, string? realLabel, string? label, WorldObject value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
