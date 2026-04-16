using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class RotateOrbitSpinTransformMapCommand : TransformRotatableMapObjectCommand
    {
        [ObservableProperty]
        private double rotation;

        [ObservableProperty]
        private bool isRotationOrbit, isRotationSpin;
        private double rotationBefore;
        private readonly double centerX;
        private readonly double centerY;

        public RotateOrbitSpinTransformMapCommand(IMultiRotatableMapObject multiRotatableMapObject) : base(multiRotatableMapObject)
        {
            rotationBefore = rotation;
            isRotationOrbit = isRotationSpin = true;
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            var targets = multiRotatableMapObject.GetSelectedRotatableMapObjects();
            foreach (var item in targets)
            {
                if (item.X < minX)
                    minX = item.X;
                if (item.X > maxX)
                    maxX = item.X;
                if (item.Y < minY)
                    minY = item.Y;
                if (item.Y > maxY)
                    maxY = item.Y;
            }
            centerX = (minX + maxX) / 2.0;
            centerY = (minY + maxY) / 2.0;
        }

        public override void Undo()
        {
            (Rotation, rotationBefore) = (rotationBefore, Rotation);
            base.Undo();
        }

        public override void Redo()
        {
            base.Redo();
            (Rotation, rotationBefore) = (rotationBefore, Rotation);
        }

        public override void Apply()
        {
            if (IsRotationOrbit)
                if (IsRotationSpin)
                    RotateOrbitSpin();
                else
                    RotateOrbitNoSpin();
            else
                if (IsRotationSpin)
                    RotateNoOrbitSpin();
                else
                    RotateNoOrbitNoSpin();
        }

        private void RotateOrbitSpin()
        {
            double rotationRad = Rotation * Math.PI / 180.0;
            double cos = Math.Cos(rotationRad);
            double sin = Math.Sin(rotationRad);

            foreach (var kv in _before)
            {
                double dx = kv.Value.X - centerX;
                double dy = kv.Value.Y - centerY;

                double rotatedX = dx * cos - dy * sin;
                double rotatedY = dx * sin + dy * cos;

                kv.Key.X = centerX + rotatedX;
                kv.Key.Y = centerY + rotatedY;

                kv.Key.ZRotation = kv.Value.ZRotation + Rotation;
            }

            double multiDx = multiXBefore - centerX;
            double multiDy = multiYBefore - centerY;

            double multiRotatedX = multiDx * cos - multiDy * sin;
            double multiRotatedY = multiDx * sin + multiDy * cos;

            multiRotatableMapObject.X = centerX + multiRotatedX;
            multiRotatableMapObject.Y = centerY + multiRotatedY;

            multiRotatableMapObject.ZRotation = multiZRotationBefore + Rotation;
        }

        private void RotateOrbitNoSpin()
        {
            double rotationRad = Rotation * Math.PI / 180.0;
            double cos = Math.Cos(rotationRad);
            double sin = Math.Sin(rotationRad);

            foreach (var kv in _before)
            {
                double dx = kv.Value.X - centerX;
                double dy = kv.Value.Y - centerY;

                double rotatedX = dx * cos - dy * sin;
                double rotatedY = dx * sin + dy * cos;

                kv.Key.X = centerX + rotatedX;
                kv.Key.Y = centerY + rotatedY;

                kv.Key.ZRotation = kv.Value.ZRotation;
            }

            double multiDx = multiXBefore - centerX;
            double multiDy = multiYBefore - centerY;

            double multiRotatedX = multiDx * cos - multiDy * sin;
            double multiRotatedY = multiDx * sin + multiDy * cos;

            multiRotatableMapObject.X = centerX + multiRotatedX;
            multiRotatableMapObject.Y = centerY + multiRotatedY;

            multiRotatableMapObject.ZRotation = multiZRotationBefore;
        }

        private void RotateNoOrbitSpin()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;

                kv.Key.ZRotation = kv.Value.ZRotation + Rotation;
            }

            multiRotatableMapObject.X = multiXBefore;
            multiRotatableMapObject.Y = multiYBefore;

            multiRotatableMapObject.ZRotation = multiZRotationBefore + Rotation;
        }

        private void RotateNoOrbitNoSpin()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X;
                kv.Key.Y = kv.Value.Y;

                kv.Key.ZRotation = kv.Value.ZRotation;
            }

            multiRotatableMapObject.X = multiXBefore;
            multiRotatableMapObject.Y = multiYBefore;

            multiRotatableMapObject.ZRotation = multiZRotationBefore;
        }
    }
}
