using System.Collections.ObjectModel;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldObservableCollection : RuleField<ObservableCollection<RuleField>>
    {
        public RuleFieldObservableCollection(string? label, ObservableCollection<RuleField> value, bool isOptional = true, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
