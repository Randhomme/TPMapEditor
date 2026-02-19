using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces.Implementations
{
    public class AlignTransformMapCommand : TransformMovableMapObjectCommand
    {
        public bool AlignOnX { get; set; } = false;
        public bool AlignOnY { get; set; } = false;
        public bool AlignOnZ { get; set; } = false;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;

        public AlignTransformMapCommand(IEnumerable<IMovableMapObject> targets) : base(targets)
        {
        }

        public override void Apply()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = AlignOnX ? X : kv.Value.X;
                kv.Key.Y = AlignOnY ? Y : kv.Value.Y;
                kv.Key.Z = AlignOnZ ? Z : kv.Value.Z;
            }
        }
    }
}
