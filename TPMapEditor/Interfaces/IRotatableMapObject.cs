using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Map object that you can rotate in 3D
    /// </summary>
    public interface IRotatableMapObject : IMovableMapObject
    {
        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }
    }
}
