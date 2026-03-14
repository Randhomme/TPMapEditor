using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldBannerType : RuleField<BannerType>
    {
        public RuleFieldBannerType(WorldMap map, string? realLabel, string? label, BannerType value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value.GetName()}'";
        }
    }
}
