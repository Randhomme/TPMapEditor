using System.Collections.Generic;

namespace TPMapEditor.Interfaces
{
    public interface IMultiRotatableMapObject : IMultiMovableMapObject
    {
        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }
        public void BeginSpinXFixTransformMapCommand();
        public void BeginSpinYFixTransformMapCommand();
        public void BeginSpinZFixTransformMapCommand();
        public void UpdateSpinFixTransformMapCommand();
        public void EndSpinFixTransformMapCommand();
        public IEnumerable<IRotatableMapObject> GetSelectedRotatableMapObjects();
    }
}
