using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;

namespace TPMapEditor.Data
{
    public partial class WorldObject : SelectableMapObject
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
        private bool hasGroup, hasPlayer;

        public WorldMap Map { get; }

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

        public WorldObject(WorldMap map)
        {
            Map = map;
            type = WorldObjectType.WotTypes.FirstOrDefault();
            this.id = nextId++;
        }

        public WorldObject(WorldMap map, WorldObjectType type, double x, double y, double zRotation)
        {
            Map = map;
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
                if (!HasGroup)
                    HasGroup = true;
            }
            else if (HasGroup)
            {
                HasGroup = false;
            }
        }

        partial void OnHasGroupChanged(bool value)
        {
            if (value)
            {
                Group ??= Map.Groups.FirstOrDefault();
            }
            else
            {
                Group = null;
            }
        }

        partial void OnPlayerChanged(Player? value)
        {
            if(value == null)
            {
                if (HasPlayer)
                    HasPlayer = false;
            }
            else
            {
                if (!HasPlayer)
                    HasPlayer = true;
            }
        }

        partial void OnHasPlayerChanged(bool value)
        {
            if (value)
            {
                Player ??= Map.Players.FirstOrDefault();
            }
            else
            {
                Player = null;
            }
        }

        public override string ToString() => $"#{Id} {Type}";

        public static void ResetNextId() => nextId = 0;
    }
}
