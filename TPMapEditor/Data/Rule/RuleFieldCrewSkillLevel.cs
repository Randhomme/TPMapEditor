using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldCrewSkillLevel : RuleField<CrewSkillLevel>
    {
        public RuleFieldCrewSkillLevel(WorldMap map, string? realLabel, string? label, CrewSkillLevel value = CrewSkillLevel.CREWSKILLLEVEL, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
