using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldInt : RuleField<int>
    {
        [ObservableProperty]
        private int min, max;

        public RuleFieldInt(string? label = null, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
            this.min = min;
            this.max = max;
        }
    }
}
