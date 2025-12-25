using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
{
    public abstract partial class NamedMapObject : ObservableObject, INamedMapObject
    {
        [ObservableProperty]
        private string name;

        partial void OnNameChanging(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{GetType().Name} name cannot be null or empty.");
            if (IsDefaultName(value))
                throw new ArgumentException($"{value} is already a default value for {GetType().Name}.");
            if (IsNameTaken(value))
                throw new ArgumentException($"A {GetType().Name} with the same name in the same group already exists.");
        }

        public WorldMap Map { get; }

        public NamedMapObject(WorldMap map, string name)
        {
            Map = map;
            this.name = name;
        }

        protected abstract bool IsNameTaken(string name);

        public virtual bool IsDefaultName(string name)
        {
            return false;
        }

        public static string GenerateName(string prefix, IEnumerable<NamedMapObject> collection)
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

    public abstract partial class SelectableMapObject : ObservableObject, ISelectableMapObject
    {
        [ObservableProperty]
        private bool isSelected, isLastSelected, isShownOnUi = true;
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
