using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class ObjectivePoint : SelectableNamedMapObject, IMovableMapObject
    {
        public static string DefaultName => "NO OBJECTIVE POINT";

        public static ObjectivePoint DefaultObjectivePoint { get; } = new(null, DefaultName);

        [ObservableProperty]
        private double x, y, z;

        public ObjectivePoint(WorldMap map) : base(map, GenerateName("ObjectivePoint", map.ObjectivePoints)) { }

        public ObjectivePoint(WorldMap map, string name, double x = 0, double y = 0, double z = 0) : base(map, name)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.ObjectivePoints)
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
            var copy = (ObjectivePoint)base.Copy();
            copy.Name = GenerateName($"{this.Name}_", Map.ObjectivePoints);
            return copy;
        }
    }
}
