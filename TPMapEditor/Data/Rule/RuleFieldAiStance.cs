using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldAiStance : RuleField<AiStance>
    {
        public RuleFieldAiStance(WorldMap map, string? realLabel, string? label, AiStance value = AiStance.AISTANCE, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
