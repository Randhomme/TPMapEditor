using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace TPMapEditor.Data
{
    public partial class Group : NamedElement
    {
        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        [ObservableProperty]
        private bool canBeRemoved; //used for Player0 Group
        public ObservableCollection<WorldObject> WorldObjects { get; }

        public Group(WorldMap map, string name) : base(map, name)
        {
            Color = Colors.Black;
            WorldObjects = new ObservableCollection<WorldObject>();
            CanBeRemoved = true;
        }

        public void ClearWot()
        {
            foreach (var wot in WorldObjects)
            {
                wot.Group = null;
            }
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.Groups)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
