using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class ObjectivePoint : NamedElement
    {
        [ObservableProperty]
        private double x, y, z;

        public ObjectivePoint(WorldMap map, string name) : base(map, name)
        {
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.ObjectivePoints)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
