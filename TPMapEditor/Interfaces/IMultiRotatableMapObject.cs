using System.Collections.Generic;

namespace TPMapEditor.Interfaces
{
    public interface IMultiRotatableMapObject : IMultiMovableMapObject
    {
        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }
        public void BeginSpinXFixTransformMapCommand(double multiXRotationBefore);
        public void BeginSpinYFixTransformMapCommand(double multiYRotationBefore);
        public void BeginSpinZFixTransformMapCommand(double multiZRotationBefore);
        public void UpdateSpinFixTransformMapCommand();
        public void EndSpinFixTransformMapCommand();
        public IEnumerable<IRotatableMapObject> GetSelectedRotatableMapObjects();
    }
}
