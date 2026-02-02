using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Interfaces;
using TPMapEditor.Utils;

namespace TPMapEditor.Data.Rule
{
    public abstract partial class RuleField : CustomObservableValidator, ICopiableMapObject
    {
        [ObservableProperty]
        private string? realLabel, label; //realLabel is from map file, label is displayed text

        [ObservableProperty]
        private bool isOptional;

        [ObservableProperty]
        private bool isShown = true;

        [ObservableProperty]
        private string? optionalLabel;

        public WorldMap Map { get; }

        protected RuleField(WorldMap map, string? realLabel, string? label, bool isOptional, string? optionalLabel, bool isShown)
        {
            Map = map;
            this.realLabel = realLabel;
            this.label = label;
            this.isOptional = isOptional;
            this.optionalLabel = optionalLabel;
            this.isShown = isShown;
        }

        public void AddWeakPropertyChangedHandler(EventHandler<PropertyChangedEventArgs> handler)
        {
            PropertyChangedEventManager.AddHandler(this, handler, string.Empty);
        }

        public abstract ICopiableMapObject Copy();
    }

    public abstract partial class RuleField<T> : RuleField
    {

        [ObservableProperty]
        [property: Required]
        private T? value;

        protected RuleField(WorldMap map, string? realLabel, string? label, T? value, bool isOptional, string? optionalLabel, bool isShown) : base(map, realLabel, label, isOptional, optionalLabel, isShown)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value}'";
        }

        public override ICopiableMapObject Copy()
        {
            return (ICopiableMapObject)this.MemberwiseClone();
        }
    }
}
