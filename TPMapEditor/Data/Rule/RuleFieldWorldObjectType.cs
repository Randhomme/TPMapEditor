using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldWorldObjectType : RuleField<WorldObjectType>
    {
        public RuleFieldWorldObjectType(string? label = null, WorldObjectType value = WorldObjectType.Asteroid, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
