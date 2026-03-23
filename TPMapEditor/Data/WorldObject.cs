using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class WorldObject : SelectableMapObject, IMovableMapObject, IRotatableMapObject
    {
        private static int nextId = 0;

        [ObservableProperty]
        [property: Required(ErrorMessage = "The WorldObjectType is required.")]
        private WorldObjectType type;
        [ObservableProperty]
        private double x, y, z, xRotation = 90, yRotation, displayedZRotation; //rotation Euler XYZ (by default in Blender)
        [ObservableProperty]
        private Group? group;
        [ObservableProperty]
        private Player? player;
        [ObservableProperty]
        private bool hasGroup, hasPlayer;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasState))]
        private string aIEntity = "\t\tType String ''", renderEntity = "\t\tType String ''", physicsEntity = "\t\tType String ''", collisionEntity = "\t\tType String ''", customInfoEntity = "\t\tType String ''";

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

        private double zRotation;
        public double ZRotation
        {
            get => zRotation;
            set
            {
                zRotation = value;
                DisplayedZRotation = -value;
            }
        }

        public bool HasState { get => !CheckState(); }

        private bool CheckState()
        {
            return AIEntity.Trim() == "Type String ''"
                && RenderEntity.Trim() == "Type String ''"
                && PhysicsEntity.Trim() == "Type String ''"
                && PhysicsEntity.Trim() == "Type String ''"
                && CollisionEntity.Trim() == "Type String ''"
                && CustomInfoEntity.Trim() == "Type String ''";
        }

        public WorldObject(WorldMap map) : base(map)
        {
            type = WorldObjectType.WotTypes.FirstOrDefault();
            this.id = nextId++;
        }

        public WorldObject(WorldMap map, WorldObjectType type, double x, double y, double z, double zRotation) : base(map)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = z;
            this.ZRotation = zRotation;
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

        public override ICopiableMapObject Copy()
        {
            var copy = (WorldObject)base.Copy();
            copy.Group?.WorldObjects.Add(copy);
            copy.Id = nextId++;
            return copy;
        }

        public static void ResetNextId() => nextId = 0;
    }
}
