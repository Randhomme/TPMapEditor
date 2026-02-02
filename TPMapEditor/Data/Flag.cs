using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class Flag : SelectableNamedMapObject
    {
        [ObservableProperty]
        private bool value;

        public Flag(WorldMap map) : base(map, GenerateName("Flag", map.Flags)) { }

        public Flag(WorldMap map, string name, bool value = false) : base(map, name)
        {
            Value = value;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.Flags)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override ICopiableMapObject Copy()
        {
            var copy = (Flag)base.Copy();
            copy.Name = GenerateName($"{Name}_", Map.Flags);
            return copy;
        }
    }
}
