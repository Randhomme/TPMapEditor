using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class Flag : NamedElement
    {
        [ObservableProperty]
        private bool value;

        public Flag(WorldMap map, string name, bool value = false) : base(map, name)
        {
            Value = value;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.Flags)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
