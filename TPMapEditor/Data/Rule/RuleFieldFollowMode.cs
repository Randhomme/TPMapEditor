using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldFollowMode : RuleField<FollowMode>
    {
        public RuleFieldFollowMode(string? label = null, FollowMode value = FollowMode.ToEnd, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
