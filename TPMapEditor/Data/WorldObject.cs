using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class WorldObject : DefaultElement
    {
        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private WotGridItem type;
        [ObservableProperty]
        private double x, y, z, xRotation, yRotation, zRotation;
        [ObservableProperty]
        private Group? group;
        [ObservableProperty]
        private Player? player;

        public WorldObject(WotGridItem type, double x, double y, double zRotation)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = this.xRotation = this.yRotation = 0;
            this.zRotation = zRotation;
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
    }
}
