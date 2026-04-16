using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class RotateOrbitTransformMapCommand : TransformMovableMapObjectCommand
    {
        [ObservableProperty]
        private double rotation;
        private double rotationBefore;
        private readonly double centerX;
        private readonly double centerY;

        public RotateOrbitTransformMapCommand(IMultiMovableMapObject multiMovableMapObject) : base(multiMovableMapObject)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            var targets = multiMovableMapObject.GetSelectedMovableMapObjects();
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
                double dx = kv.Value.X - centerX;
                double dy = kv.Value.Y - centerY;

                double rotatedX = dx * cos - dy * sin;
                double rotatedY = dx * sin + dy * cos;

                kv.Key.X = centerX + rotatedX;
                kv.Key.Y = centerY + rotatedY;
            }
            double multiDx = multiXBefore - centerX;
            double multiDy = multiYBefore - centerY;

            double multiRotatedX = multiDx * cos - multiDy * sin;
            double multiRotatedY = multiDx * sin + multiDy * cos;

            multiMovableMapObject.X = centerX + multiRotatedX;
            multiMovableMapObject.Y = centerY + multiRotatedY;
        }
    }
}
