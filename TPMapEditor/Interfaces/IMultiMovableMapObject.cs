using System.Collections.Generic;

namespace TPMapEditor.Interfaces
{
    public interface IMultiMovableMapObject : IMultiSelectableMapObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public void BeginAlignXTransformMapCommand();
        public void BeginAlignYTransformMapCommand();
        public void BeginAlignZTransformMapCommand();
        public void UpdateAlignTransformMapCommand();
        public void EndAlignTransformMapCommand();
        public IEnumerable<IMovableMapObject> GetSelectedMovableMapObjects();
    }
}
