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
        public static string DefaultName => "NO OBJECTIVE POINT";

        [ObservableProperty]
        private double x, y, z;
        [ObservableProperty]
        private bool isSelected, isLastSelected;

        public ObjectivePoint(WorldMap map, string name, double x, double y, double z = 0) : base(map, name)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }
    }
}
