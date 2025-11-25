using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public abstract class NamedElement : ObservableObject
    {
        private string name;
        protected WorldMap map;

        public string Name
        {
            get => name;
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentException($"{GetType().Name} name cannot be null or empty.");
                if (IsNameTaken(value)) throw new ArgumentException("A " + this.GetType().Name.ToLowerInvariant() + " with the same name already exists.");
                name = value;
                OnPropertyChanged();
            }
        }

        public NamedElement(WorldMap map, string name)
        {
            this.map = map;
            this.name = name;
        }

        protected abstract bool IsNameTaken(string name);

        public static string GenerateName(string prefix, IEnumerable<NamedElement> collection)
        {
            var c = 0;
            foreach (var namedElement in collection)
            {
                if (namedElement.Name.Length >= prefix.Length)
                {
                    var s = namedElement.Name.Substring(prefix.Length);
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
}
