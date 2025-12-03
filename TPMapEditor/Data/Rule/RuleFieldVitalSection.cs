using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldVitalSection : RuleField<VitalSection>
    {
        public RuleFieldVitalSection(string? realLabel, string? label, VitalSection value = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
