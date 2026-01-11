using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Interfaces;
using TPMapEditor.Utils;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract partial class NamedObject : CustomObservableValidator, INamedObject
    {
        private string name;

        [Required(ErrorMessage = "The name cannot be empty.", AllowEmptyStrings = false)]
        [CustomValidation(typeof(NamedObject), nameof(ValidateName))]
        public string Name
        {
            get => name;
            set => SetAndValidateProperty(ref name, value);
        }

        public NamedObject(string name)
        {
            this.name = name;
        }

        public static ValidationResult ValidateName(object value, ValidationContext context)
        {
            var instance = (NamedObject)context.ObjectInstance;
            if (instance.IsNameTaken(value.ToString()))
                return new ValidationResult($"A {instance.GetType().Name} with the same name already exists.");
            if (instance.IsDefaultName(value.ToString()))
                return new ValidationResult($"{value} is already a default name for {instance.GetType().Name}.");
            return ValidationResult.Success;
        }

        protected abstract bool IsNameTaken(string name);

        public virtual bool IsDefaultName(string name)
        {
            return false;
        }

        public static string GenerateName(string prefix, IEnumerable<INamedObject> collection)
        {
            int i = prefix.Length - 1;
            while (i >= 0 && char.IsDigit(prefix[i]))
            {
                i--;
            }

            if (i < prefix.Length - 1)
            {
                prefix = prefix.Substring(0, i + 1);
            }
            var c = 0;
            foreach (var namedMapObject in collection)
            {
                if (namedMapObject.Name.Length >= prefix.Length)
                {
                    var s = namedMapObject.Name.Substring(prefix.Length);
                    if (int.TryParse(s, out int j))
                    {
                        if (j > c) c = j;
                    }
                }
            }
            return prefix + (c + 1);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
