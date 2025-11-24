namespace TPMapEditor.Data.Rule
{
    public class RuleFieldTimer : RuleField<Timer>
    {
        public RuleFieldTimer(string? realLabel, string? label, Timer value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
