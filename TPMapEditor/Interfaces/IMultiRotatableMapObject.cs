using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
