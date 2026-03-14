using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract class TransformRotatableMapObjectCommand : ObservableObject, IUndoableMapCommand
    {
        protected readonly Dictionary<IRotatableMapObject, (double X, double Y, double Z, double ZRotation)> _before;
        protected Dictionary<IRotatableMapObject, (double X, double Y, double Z, double ZRotation)>? _after;

        public bool CanUndo { get; private set; } = true;

        public TransformRotatableMapObjectCommand(IEnumerable<IRotatableMapObject> targets)
        {
            _before = targets.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z, o.ZRotation));
        }

        public abstract void Apply();

        private void Commit()
        {
            _after = _before.Keys.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z, o.ZRotation));
        }

        public virtual void Undo()
        {
            this.Commit();
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;
                kv.Key.Z = kv.Value.Z;
                kv.Key.ZRotation = kv.Value.ZRotation;
            }
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
                    kv.Key.ZRotation = kv.Value.ZRotation;
                }
            }
            CanUndo = true;
        }
    }
}
