using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Interfaces.Implementations
{
    /// <summary>
    /// Base class for a transformation command on a <see cref="IMovableMapObject"/>. Derive this class to implement a command action.
    /// </summary>
    public abstract class TransformMovableMapObjectCommand : ObservableObject, IUndoableMapCommand
    {
        protected readonly Dictionary<IMovableMapObject, (double X, double Y, double Z)> _before;
        protected Dictionary<IMovableMapObject, (double X, double Y, double Z)>? _after;
        protected IMultiMovableMapObject multiMovableMapObject;
        protected double multiXBefore, multiYBefore, multiZBefore;

        public bool CanUndo { get; private set; } = true;

        public TransformMovableMapObjectCommand(IMultiMovableMapObject multiMovableMapObject)
        {
            this.multiMovableMapObject = multiMovableMapObject;
            multiXBefore = multiMovableMapObject.X;
            multiYBefore = multiMovableMapObject.Y;
            multiZBefore = multiMovableMapObject.Z;
            _before = multiMovableMapObject.GetSelectedMovableMapObjects().ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z));
        }

        public abstract void Apply();

        private void Commit()
        {
            _after = _before.Keys.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z));
        }

        public virtual void Undo()
        {
            this.Commit();
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;
                kv.Key.Z = kv.Value.Z;
            }
            multiMovableMapObject.UseUpdateCommands = false;
            (multiMovableMapObject.X, multiXBefore) = (multiXBefore, multiMovableMapObject.X);
            (multiMovableMapObject.Y, multiYBefore) = (multiYBefore, multiMovableMapObject.Y);
            (multiMovableMapObject.Z, multiZBefore) = (multiZBefore, multiMovableMapObject.Z);
            multiMovableMapObject.UseUpdateCommands = true;
            CanUndo = false;
        }

        public virtual void Redo()
        {
            if (_after != null)
            {
                foreach (var kv in _after)
                {
                    kv.Key.X = kv.Value.X;
                    kv.Key.Y = kv.Value.Y;
                    kv.Key.Z = kv.Value.Z;
                }
            }
            multiMovableMapObject.UseUpdateCommands = false;
            (multiMovableMapObject.X, multiXBefore) = (multiXBefore, multiMovableMapObject.X);
            (multiMovableMapObject.Y, multiYBefore) = (multiYBefore, multiMovableMapObject.Y);
            (multiMovableMapObject.Z, multiZBefore) = (multiZBefore, multiMovableMapObject.Z);
            multiMovableMapObject.UseUpdateCommands = true;
            CanUndo = true;
        }
    }
}
