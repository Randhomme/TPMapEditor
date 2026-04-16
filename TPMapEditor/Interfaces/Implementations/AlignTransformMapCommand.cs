using CommunityToolkit.Mvvm.ComponentModel;

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

        public AlignTransformMapCommand(IMultiMovableMapObject multiMovableMapObject, bool is3D) : base(multiMovableMapObject)
        {
            xBefore = x;
            yBefore = y;
            zBefore = z;
            Is3D = is3D;
        }

        public AlignTransformMapCommand(IMultiMovableMapObject multiMovableMapObject, double multiXBefore, double multiYBefore, double multiZBefore, bool is3D) : base(multiMovableMapObject, multiXBefore, multiYBefore, multiZBefore)
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
            multiMovableMapObject.X = AlignOnX ? X : multiXBefore;
            multiMovableMapObject.Y = AlignOnY ? Y : multiYBefore;
            multiMovableMapObject.Z = AlignOnZ ? Z : multiZBefore;
        }
    }
}
