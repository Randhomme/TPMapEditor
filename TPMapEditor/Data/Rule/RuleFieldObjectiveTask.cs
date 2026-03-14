namespace TPMapEditor.Data.Rule
{
    public class RuleFieldObjectiveTask : RuleField<ObjectiveTask>
    {
        public RuleFieldObjectiveTask(WorldMap map, string? realLabel, string? label, ObjectiveTask value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
