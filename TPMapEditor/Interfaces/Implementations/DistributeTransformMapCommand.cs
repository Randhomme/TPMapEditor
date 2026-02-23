using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class DistributeTransformMapCommand : TransformMovableMapObjectCommand
    {
        [ObservableProperty]
        private bool distributeOnX, distributeOnY, distributeOnZ;

        [ObservableProperty]
        private double x, y, z;

        private double startX, startY, startZ, xBefore, yBefore, zBefore;

        public bool Is3D { get; }

        public DistributeTransformMapCommand(IEnumerable<IMovableMapObject> targets, bool is3D) : base(targets)
        {
            xBefore = x;
            yBefore = y;
            zBefore = z;
            Is3D = is3D;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            foreach (var item in targets)
            {
                if (item.X < minX)
                    minX = item.X;
                if (item.Y < minY)
                    minY = item.Y;
                if (item.Z < minZ)
                    minZ = item.Z;
            }
            startX = minX;
            startY = minY;
            startZ = minZ;
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
            int i = 0;
            foreach (var kv in _before)
            {
                kv.Key.X = DistributeOnX ? startX + i * X : kv.Value.X;
                kv.Key.Y = DistributeOnY ? startY + i * Y : kv.Value.Y;
                kv.Key.Z = DistributeOnZ ? startZ + i * Z : kv.Value.Z;
                i++;
            }
        }
    }
}
