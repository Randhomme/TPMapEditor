namespace TPMapEditor.Data.Rule
{
    public class RuleFieldTimer : RuleField<Timer>
    {
        public RuleFieldTimer(string? label, Timer value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
