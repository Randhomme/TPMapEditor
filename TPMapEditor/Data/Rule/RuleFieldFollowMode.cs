using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldFollowMode : RuleField<FollowMode>
    {
        public RuleFieldFollowMode(WorldMap map, string? realLabel, string? label, FollowMode value = FollowMode.ToEnd, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
