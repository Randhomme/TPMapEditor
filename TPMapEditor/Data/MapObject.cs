using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using TPMapEditor.Interfaces;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public abstract partial class MapObject : CustomObservableValidator, IMapObject
    {
        public WorldMap Map { get; }
        protected MapObject(WorldMap map)
        {
            Map = map;
        }
    }

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
                return new ValidationResult($"A {instance.GetType().Name.ToLowerInvariant()} with the same name already exists.");
            if (instance.IsDefaultName(value.ToString()))
                return new ValidationResult($"A {value} is already a default name for {instance.GetType().Name}.");
            return ValidationResult.Success;
        }        

        protected abstract bool IsNameTaken(string name);

        public virtual bool IsDefaultName(string name)
        {
            return false;
        }

        public static string GenerateName(string prefix, IEnumerable<INamedObject> collection)
        {
            var c = 0;
            foreach (var namedMapObject in collection)
            {
                if (namedMapObject.Name.Length >= prefix.Length)
                {
                    var s = namedMapObject.Name.Substring(prefix.Length);
                    if (int.TryParse(s, out int i))
                    {
                        if (i > c) c = i;
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

    public abstract partial class NamedMapObject : NamedObject, IMapObject
    {
        public WorldMap Map { get; }
        protected NamedMapObject(WorldMap map, string name) : base(name)
        {
            Map = map;
        }
    }

    public abstract partial class SelectableMapObject : MapObject, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;

        protected SelectableMapObject(WorldMap map) : base(map)
        {
        }
    }

    public abstract partial class SelectableNamedMapObject : NamedMapObject, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;

        protected SelectableNamedMapObject(WorldMap map, string name) : base(map, name)
        {
        }
    }
}
