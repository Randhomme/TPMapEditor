using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiWorldObjectViewModel : MultiRotatableMapObjectViewModel<WorldObject>
    {
        [ObservableProperty]
        private WorldObjectType type;
        [ObservableProperty]
        private Group? group;
        [ObservableProperty]
        private Player? player;
        [ObservableProperty]
        private bool hasGroup, hasPlayer;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasState))]
        private string aIEntity, renderEntity, physicsEntity, collisionEntity, customInfoEntity;

        public WorldMap Map { get; }

        public MultiWorldObjectViewModel(IEnumerable<WorldObject> selectedMapObjects, IUndoManagerService undoManagerService, WorldMap map) : base(selectedMapObjects, undoManagerService)
        {
            type = WorldObjectType.WotTypes.FirstOrDefault();
            aIEntity = renderEntity = physicsEntity = collisionEntity = customInfoEntity = string.Empty;
            Map = map;
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

        protected override void UpdateFromMapObject_Internal(WorldObject mapObject)
        {
            base.UpdateFromMapObject_Internal(mapObject);
            Type = mapObject.Type;
            Group = mapObject.Group;
            HasGroup = mapObject.HasGroup;
            Player = mapObject.Player;
            HasPlayer = mapObject.HasPlayer;
            AIEntity = mapObject.AIEntity;
            RenderEntity = mapObject.RenderEntity;
            PhysicsEntity = mapObject.PhysicsEntity;
            CollisionEntity = mapObject.CollisionEntity;
            CustomInfoEntity = mapObject.CustomInfoEntity;
        }

        partial void OnTypeChanged(WorldObjectType? value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Type = Type;
                }
            }
        }

        partial void OnAIEntityChanged(string value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.AIEntity = AIEntity;
                }
            }
        }

        partial void OnRenderEntityChanged(string value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.RenderEntity = RenderEntity;
                }
            }
        }

        partial void OnPhysicsEntityChanged(string value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.PhysicsEntity = PhysicsEntity;
                }
            }
        }

        partial void OnCollisionEntityChanged(string value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.CollisionEntity = CollisionEntity;
                }
            }
        }

        partial void OnCustomInfoEntityChanged(string value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.CustomInfoEntity = CustomInfoEntity;
                }
            }
        }

        partial void OnGroupChanged(Group? value)
        {
            if (UseUpdateCommands)
            {
                if (value != null)
                {
                    HasGroup = true;
                }
                else
                {
                    HasGroup = false;
                }
                foreach (var item in selectedMapObjects)
                {
                    item.Group = value;
                }
            }
        }

        partial void OnHasGroupChanged(bool value)
        {
            if (UseUpdateCommands)
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
        }

        partial void OnPlayerChanged(Player? value)
        {
            if (UseUpdateCommands)
            {
                if (value != null)
                {
                    HasPlayer = true;
                }
                else
                {
                    HasPlayer = false;
                }
                foreach (var item in selectedMapObjects)
                {
                    item.Player = value;
                }
            }
        }

        partial void OnHasPlayerChanged(bool value)
        {
            if (UseUpdateCommands)
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
        }
    }
}
