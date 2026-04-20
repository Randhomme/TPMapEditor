using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TPMapEditor.Utils
{
    /// <summary>
    /// An observable validator that triggers exceptions instead of binding errors, using Validator.ValidateProperty and Validator.ValidateObject
    /// </summary>
    public class CustomObservableValidator : ObservableObject
    {
        protected void ValidateProperty<T>(T value, [CallerMemberName] string? propertyName = null)
        {
            Validator.ValidateProperty(value, new(this) { MemberName = propertyName });
        }

        public virtual void ValidateAllProperties()
        {
            CustomValidator.ValidateAllProperties(this);
        }

        protected void SetAndValidateProperty<T>(ref T field, T newValue, [CallerMemberName]string? propertyName = null)
        {
            ValidateProperty(newValue, propertyName);
            SetProperty(ref field, newValue, propertyName);
        }
    }

    public static class CustomValidator
    {
        public static void ValidateAllProperties(object instance)
        {
            Validator.ValidateObject(instance, new(instance), true);
        }
    }
}
