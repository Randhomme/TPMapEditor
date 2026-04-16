using System.Collections.Generic;

namespace TPMapEditor.Interfaces
{
    public interface IMultiMovableMapObject : IMultiSelectableMapObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public void BeginAlignXTransformMapCommand(double multiXBefore);
        public void BeginAlignYTransformMapCommand(double multiYBefore);
        public void BeginAlignZTransformMapCommand(double multiZBefore);
        public void UpdateAlignTransformMapCommand();
        public void EndAlignTransformMapCommand();
        public IEnumerable<IMovableMapObject> GetSelectedMovableMapObjects();
    }
}
