using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public abstract partial class RuleField : ObservableObject
    {
        [ObservableProperty]
        private string? realLabel, label; //realLabel is from map file, label is displayed text

        [ObservableProperty]
        private bool isOptional;

        [ObservableProperty]
        private bool isShown = true;

        [ObservableProperty]
        private string? optionalLabel;

        protected RuleField(string? realLabel, string? label, bool isOptional, string? optionalLabel, bool isShown)
        {
            this.realLabel = realLabel;
            this.label = label;
            this.isOptional = isOptional;
            this.optionalLabel = optionalLabel;
            this.isShown = isShown;
        }
    }
    public abstract partial class RuleField<T> : RuleField
    {

        [ObservableProperty]
        private T value;

        partial void OnValueChanged(T? oldValue, T newValue)
        {
            
        }

        protected RuleField(string? realLabel, string? label, T value, bool isOptional, string? optionalLabel, bool isShown) : base(realLabel, label, isOptional, optionalLabel, isShown)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }
    }
}
