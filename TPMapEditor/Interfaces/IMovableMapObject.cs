using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Movable map object in 2D
    /// </summary>
    public interface IMovableMapObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
