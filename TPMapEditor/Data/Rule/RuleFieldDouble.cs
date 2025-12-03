using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleFieldDouble : RuleField<double>
    {
        [ObservableProperty]
        private double min, max;

        public RuleFieldDouble(string? realLabel, string? label, double value = 0, double min = -9999, double max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            return $"{RealLabel} {Value:0.000000}";
        }
    }
}
