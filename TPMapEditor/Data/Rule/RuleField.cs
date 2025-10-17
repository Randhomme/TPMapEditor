using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data.Rule
{
    public abstract partial class RuleField : ObservableObject
    {
        [ObservableProperty]
        private string? label;

        [ObservableProperty]
        private bool isOptional;

        [ObservableProperty]
        private bool isShown = true;

        [ObservableProperty]
        private string? optionalLabel;

        protected RuleField(string? label = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
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

        protected RuleField(string? label, T value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, isOptional, optionalLabel, isShown)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }
    }
}
