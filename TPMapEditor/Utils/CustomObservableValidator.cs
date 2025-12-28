using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        protected void SetAndValidateProperty<T>(ref T field, T newValue, [CallerMemberName]string? propertyName = null)
        {
            ValidateProperty(newValue, propertyName);
            SetProperty(ref field, newValue, propertyName);
        }
    }

    public static class CustomValidator
    {
        public static void ValidateAllProperties(this object instance)
        {
            Validator.ValidateObject(instance, new(instance), true);
        }
    }
}
