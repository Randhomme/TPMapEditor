using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class Group : SelectableNamedMapObject
    {
        public static string DefaultName => "Player0 Group";

        public static Group DefaultGroup { get; } = new(null, DefaultName);

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file

        public ObservableCollection<WorldObject> WorldObjects { get; }

        public Group(WorldMap map) : base(map, GenerateName("Group", map.Groups))
        {
            Color = Colors.Black;
            WorldObjects = new ObservableCollection<WorldObject>();
        }

        public Group(WorldMap map, string name) : base(map, name)
        {
            Color = Colors.Black;
            WorldObjects = new ObservableCollection<WorldObject>();
        }

        public void ClearWot()
        {
            while (WorldObjects.Count > 0)
            {
                var wot = WorldObjects[0];
                WorldObjects.RemoveAt(0);
                wot.Group = null;
            }
        }

        protected override bool IsNameTaken(string name)
        {
            if(Map!=null)
                foreach (var item in Map.Groups)
                {
                    if (item.Name == name && item != this)
                        return true;
                }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }

        public override ICopiableMapObject Copy()
        {
            var copy = new Group(Map, GenerateName($"{Name}_", Map.Groups))
            {
                Color = this.Color
            };
            foreach (var item in WorldObjects)
            {
                copy.WorldObjects.Add(item);
            }
            return copy;
        }
    }
}
