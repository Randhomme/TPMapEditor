using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces.Implementations
{
    /// <summary>
    /// Base class for a transformation command on a <see cref="IMovable3MapObject"/>. Derive this class to implement a command action.
    /// </summary>
    public abstract class TransformMapCommand : IUndoableMapCommand
    {
        protected readonly Dictionary<IMovableMapObject, (double X, double Y, double Z)> _before;
        protected Dictionary<IMovableMapObject, (double X, double Y, double Z)>? _after;

        public TransformMapCommand(IEnumerable<IMovableMapObject> targets)
        {
            _before = targets.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z));
        }

        public abstract void Apply();

        public void Commit()
        {
            _after = _before.Keys.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z));
        }

        public void Undo()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;
                kv.Key.Z = kv.Value.Z;
            }
        }

        public void Redo()
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
        }
    }
}
