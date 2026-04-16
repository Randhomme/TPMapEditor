using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    public interface IMultiMovableMapObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public void BeginAlignXTransformMapCommand();
        public void BeginAlignYTransformMapCommand();
        public void BeginAlignZTransformMapCommand();
        public void UpdateAlignTransformMapCommand();
        public void EndAlignTransformMapCommand();
    }
}
