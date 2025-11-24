namespace TPMapEditor.Data.Rule
{
    public class RuleFieldSpeechEvent : RuleField<SpeechEvent>
    {
        public RuleFieldSpeechEvent(string? realLabel, string? label, SpeechEvent value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
