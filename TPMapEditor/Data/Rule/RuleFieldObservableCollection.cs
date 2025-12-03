using System.Collections.ObjectModel;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldObservableCollection : RuleField<ObservableCollection<RuleField>>
    {
        public RuleFieldObservableCollection(string? realLabel, ObservableCollection<RuleField> value, bool isOptional = true, string? optionalLabel = null, bool isShown = true) : base(realLabel, null, value, isOptional, optionalLabel, isShown)
        {
            foreach(var field in value)
            {
                field.IsOptional = isOptional;
            }
        }

        public override string ToString()
        {
            return $"{RealLabel} '{IsShown.ToString().ToUpperInvariant()}'";
        }
    }
}
