using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Controls;

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
