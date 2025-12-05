using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class WorldObject : ObservableObject
    {
        private static int nextId = 0;

        [ObservableProperty]
        private WorldObjectType type;
        [ObservableProperty]
        private double x, y, z, xRotation, yRotation, zRotation; //rotation Euler XYZ (by default in Blender)
        [ObservableProperty]
        private Group? group;
        [ObservableProperty]
        private Player? player;
        [ObservableProperty]
        private bool isSelected, isLastSelected;

        private int id;
        public int Id //only used for data import/export
        {
            get => id;
            set 
            {
                id = value;
                if (value >= nextId)
                    nextId = value+1;
            } 
        }

        public WorldObject(WorldObjectType type, double x, double y, double zRotation)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = this.xRotation = this.yRotation = 0;
            this.zRotation = zRotation;
            this.id = nextId++;
        }

        partial void OnGroupChanged(Group? oldValue, Group? newValue)
        {
            if (oldValue != null)
            {
                oldValue.WorldObjects.Remove(this);
            }
            if (newValue != null)
            {
                newValue.WorldObjects.Add(this);
            }
        }

        public override string ToString() => $"#{Id} {Type}";

        public static void ResetNextId() => nextId = 0;
    }
}
