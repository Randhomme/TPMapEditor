using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldVitalSection : RuleField<VitalSection>
    {
        public RuleFieldVitalSection(string? label = null, VitalSection value = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
