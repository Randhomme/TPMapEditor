using System.Collections.ObjectModel;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldObservableCollection : RuleField<ObservableCollection<RuleField>>
    {
        public RuleFieldObservableCollection(string? realLabel, ObservableCollection<RuleField> value, bool isOptional = true, string? optionalLabel = null, bool isShown = true) : base(realLabel, null, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(RealLabel))
            {
                return IsShown ? "TRUE" : "FALSE";
            }
            return string.Empty;
        }
    }
}
