namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldFlag : RuleField<Flag>
    {
        public RuleFieldFlag(string? label, Flag value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
