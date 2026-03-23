using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

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

        public RotateOrbitSpinTransformMapCommand(IEnumerable<IRotatableMapObject> targets) : base(targets)
        {
            rotationBefore = rotation;
            isRotationOrbit = isRotationSpin = true;
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
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
            double rotationRad = Rotation * Math.PI / 180.0;
            double cos = Math.Cos(rotationRad);
            double sin = Math.Sin(rotationRad);

            foreach (var kv in _before)
            {
                if (IsRotationOrbit)
                {
                    double dx = kv.Value.X - centerX;
                    double dy = kv.Value.Y - centerY;

                    double rotatedX = dx * cos - dy * sin;
                    double rotatedY = dx * sin + dy * cos;

                    kv.Key.X = centerX + rotatedX;
                    kv.Key.Y = centerY + rotatedY;
                }
                else
                {
                    kv.Key.X = kv.Value.X;
                    kv.Key.Y = kv.Value.Y;
                }
                if (IsRotationSpin)
                    kv.Key.ZRotation = kv.Value.ZRotation + Rotation;
                else
                    kv.Key.ZRotation = kv.Value.ZRotation;
            }
        }
    }
}
