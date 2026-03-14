namespace TPMapEditor.Data.Rule
{
    public class RuleFieldDialogueAudio : RuleField<string>
    {
        public RuleFieldDialogueAudio(WorldMap map, string? realLabel, string? label, string value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
