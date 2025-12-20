namespace TPMapEditor.Data.Rule
{
    public class RuleFieldSpeechEvent : RuleField<SpeechEvent>
    {
        public RuleFieldSpeechEvent(WorldMap map, string? realLabel, string? label, SpeechEvent value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
