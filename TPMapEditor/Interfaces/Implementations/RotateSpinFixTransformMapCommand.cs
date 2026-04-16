using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class RotateSpinFixTransformMapCommand : TransformRotatableMapObjectCommand
    {
        [ObservableProperty]
        private bool rotateOnX, rotateOnY, rotateOnZ;

        [ObservableProperty]
        private double xRotation, yRotation, zRotation;

        private double xRotationBefore, yRotationBefore, zRotationBefore;

        public RotateSpinFixTransformMapCommand(IMultiRotatableMapObject multiRotatableMapObject) : base(multiRotatableMapObject)
        {
            xRotationBefore = xRotation;
            yRotationBefore = yRotation;
            zRotationBefore = zRotation;
        }

        public RotateSpinFixTransformMapCommand(IMultiRotatableMapObject multiRotatableMapObject, double multiXRotationBefore, double multiYRotationBefore, double multiZRotationBefore) : base(multiRotatableMapObject, multiXRotationBefore, multiYRotationBefore, multiZRotationBefore)
        {
            xRotationBefore = xRotation;
            yRotationBefore = yRotation;
            zRotationBefore = zRotation;
        }

        public override void Undo()
        {
            (xRotationBefore, XRotation) = (XRotation, xRotationBefore);
            (yRotationBefore, YRotation) = (YRotation, yRotationBefore);
            (zRotationBefore, ZRotation) = (ZRotation, zRotationBefore);
            base.Undo();
        }

        public override void Redo()
        {
            base.Redo();
            (xRotationBefore, XRotation) = (XRotation, xRotationBefore);
            (yRotationBefore, YRotation) = (YRotation, yRotationBefore);
            (zRotationBefore, ZRotation) = (ZRotation, zRotationBefore);
        }

        public override void Apply()
        {
            foreach (var kv in _before)
            {
                kv.Key.XRotation = RotateOnX ? XRotation : kv.Value.XRotation;
                kv.Key.YRotation = RotateOnY ? YRotation : kv.Value.YRotation;
                kv.Key.ZRotation = RotateOnZ ? ZRotation : kv.Value.ZRotation;
            }
            multiRotatableMapObject.XRotation = RotateOnX ? XRotation : multiXRotationBefore;
            multiRotatableMapObject.YRotation = RotateOnY ? YRotation : multiYRotationBefore;
            multiRotatableMapObject.ZRotation = RotateOnZ ? ZRotation : multiZRotationBefore;
        }
    }
}
