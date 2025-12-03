using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldInt : RuleField<int>
    {
        [ObservableProperty]
        private int min, max;

        public RuleFieldInt(string? realLabel, string? label, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return $"{RealLabel} {Value}";
        }
    }
}
