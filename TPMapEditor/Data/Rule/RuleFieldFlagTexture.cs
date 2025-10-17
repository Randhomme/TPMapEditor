namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldFlagTexture : RuleField<string>
    {
        public RuleFieldFlagTexture(string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
