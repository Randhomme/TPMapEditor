using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class AlignTransformMapCommand : TransformMovableMapObjectCommand
    {
        [ObservableProperty]
        private bool alignOnX, alignOnY, alignOnZ;

        [ObservableProperty]
        private double x, y, z;

        private double xBefore, yBefore, zBefore;

        public bool Is3D { get; }

        public AlignTransformMapCommand(IEnumerable<IMovableMapObject> targets, bool is3D) : base(targets)
        {
            xBefore = x;
            yBefore = y;
            zBefore = z;
            Is3D = is3D;
        }

        public override void Undo()
        {
            (xBefore, X) = (X, xBefore);
            (yBefore, Y) = (Y, yBefore);
            (zBefore, Z) = (Z, zBefore);
            base.Undo();
        }

        public override void Redo()
        {
            base.Redo();
            (xBefore, X) = (X, xBefore);
            (yBefore, Y) = (Y, yBefore);
            (zBefore, Z) = (Z, zBefore);
        }

        public override void Apply()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = AlignOnX ? X : kv.Value.X;
                kv.Key.Y = AlignOnY ? Y : kv.Value.Y;
                kv.Key.Z = AlignOnZ ? Z : kv.Value.Z;
            }
        }
    }
}
