using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            string newPrefix = prefix;
            int i = newPrefix.Length - 1;
            while (i >= 0 && char.IsDigit(newPrefix[i]))
            {
                i--;
            }

            if (i < newPrefix.Length - 1)
            {
                newPrefix = newPrefix.Substring(0, i + 1);
            }
            var c = 0;
            bool shouldKeppSameName = true;
            foreach (var namedMapObject in collection)
            {
                if (namedMapObject.Name.StartsWith(newPrefix))
                {
                    if (namedMapObject.Name.Equals(prefix))
                        shouldKeppSameName = false;
                    var s = namedMapObject.Name.Substring(newPrefix.Length);
                    if (int.TryParse(s, out int j))
                    {
                        if (j > c) c = j;
                    }
                }
            }
            return shouldKeppSameName ? prefix : newPrefix + (c + 1);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
