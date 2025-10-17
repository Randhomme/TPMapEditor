using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldCrewSkillLevel : RuleField<CrewSkillLevel>
    {
        public RuleFieldCrewSkillLevel(string? label = null, CrewSkillLevel value = CrewSkillLevel.CREWSKILLLEVEL, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
