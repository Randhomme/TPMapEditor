namespace TPMapEditor.Data.Rule
{
    public class RuleFieldSpeechEvent : RuleField<SpeechEvent>
    {
        public RuleFieldSpeechEvent(string? label, SpeechEvent value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
