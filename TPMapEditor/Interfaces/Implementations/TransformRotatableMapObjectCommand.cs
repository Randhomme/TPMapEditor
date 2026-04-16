using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract class TransformRotatableMapObjectCommand : ObservableObject, IUndoableMapCommand
    {
        protected readonly Dictionary<IRotatableMapObject, (double X, double Y, double Z, double XRotation, double YRotation, double ZRotation)> _before;
        protected Dictionary<IRotatableMapObject, (double X, double Y, double Z, double XRotation, double YRotation, double ZRotation)>? _after;
        protected readonly IMultiRotatableMapObject multiRotatableMapObject;
        protected double multiXBefore, multiYBefore, multiZBefore, multiXRotationBefore, multiYRotationBefore, multiZRotationBefore;

        public bool CanUndo { get; private set; } = true;

        public TransformRotatableMapObjectCommand(IMultiRotatableMapObject multiRotatableMapObject)
        {
            this.multiRotatableMapObject = multiRotatableMapObject;
            multiXBefore = multiRotatableMapObject.X;
            multiYBefore = multiRotatableMapObject.Y;
            multiZBefore = multiRotatableMapObject.Z;
            multiXRotationBefore = multiRotatableMapObject.XRotation;
            multiYRotationBefore = multiRotatableMapObject.YRotation;
            multiZRotationBefore = multiRotatableMapObject.ZRotation;
            _before = multiRotatableMapObject.GetSelectedRotatableMapObjects().ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z, o.XRotation, o.YRotation, o.ZRotation));
        }

        public TransformRotatableMapObjectCommand(IMultiRotatableMapObject multiRotatableMapObject, double multiXRotationBefore, double multiYRotationBefore, double multiZRotationBefore)
        {
            this.multiRotatableMapObject = multiRotatableMapObject;
            multiXBefore = multiRotatableMapObject.X;
            multiYBefore = multiRotatableMapObject.Y;
            multiZBefore = multiRotatableMapObject.Z;
            this.multiXRotationBefore = multiXRotationBefore;
            this.multiYRotationBefore = multiYRotationBefore;
            this.multiZRotationBefore = multiZRotationBefore;
            _before = multiRotatableMapObject.GetSelectedRotatableMapObjects().ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z, o.XRotation, o.YRotation, o.ZRotation));
        }

        public abstract void Apply();

        private void Commit()
        {
            _after = _before.Keys.ToDictionary(
                o => o,
                o => (o.X, o.Y, o.Z, o.XRotation, o.YRotation, o.ZRotation));
        }

        public virtual void Undo()
        {
            this.Commit();
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;
                kv.Key.Z = kv.Value.Z;
                kv.Key.XRotation = kv.Value.XRotation;
                kv.Key.YRotation = kv.Value.YRotation;
                kv.Key.ZRotation = kv.Value.ZRotation;
            }
            multiRotatableMapObject.UseUpdateCommands = false;
            (multiRotatableMapObject.X, multiXBefore) = (multiXBefore, multiRotatableMapObject.X);
            (multiRotatableMapObject.Y, multiYBefore) = (multiYBefore, multiRotatableMapObject.Y);
            (multiRotatableMapObject.Z, multiZBefore) = (multiZBefore, multiRotatableMapObject.Z);
            (multiRotatableMapObject.XRotation, multiXRotationBefore) = (multiXRotationBefore, multiRotatableMapObject.XRotation);
            (multiRotatableMapObject.YRotation, multiYRotationBefore) = (multiYRotationBefore, multiRotatableMapObject.YRotation);
            (multiRotatableMapObject.ZRotation, multiZRotationBefore) = (multiZRotationBefore, multiRotatableMapObject.ZRotation);
            multiRotatableMapObject.UseUpdateCommands = true;
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
                    kv.Key.XRotation = kv.Value.XRotation;
                    kv.Key.YRotation = kv.Value.YRotation;
                    kv.Key.ZRotation = kv.Value.ZRotation;
                }
            }
            multiRotatableMapObject.UseUpdateCommands = false;
            (multiRotatableMapObject.X, multiXBefore) = (multiXBefore, multiRotatableMapObject.X);
            (multiRotatableMapObject.Y, multiYBefore) = (multiYBefore, multiRotatableMapObject.Y);
            (multiRotatableMapObject.Z, multiZBefore) = (multiZBefore, multiRotatableMapObject.Z);
            (multiRotatableMapObject.XRotation, multiXRotationBefore) = (multiXRotationBefore, multiRotatableMapObject.XRotation);
            (multiRotatableMapObject.YRotation, multiYRotationBefore) = (multiYRotationBefore, multiRotatableMapObject.YRotation);
            (multiRotatableMapObject.ZRotation, multiZRotationBefore) = (multiZRotationBefore, multiRotatableMapObject.ZRotation);
            multiRotatableMapObject.UseUpdateCommands = true;
            CanUndo = true;
        }
    }
}
