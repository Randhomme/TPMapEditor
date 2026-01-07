using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces.Implementations
{
    internal class DistributeTransformMapCommand : TransformMapCommand
    {
        public bool DistributeOnX { get; set; } = false;
        public bool DistributeOnY { get; set; } = false;
        public bool DistributeOnZ { get; set; } = false;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;

        public DistributeTransformMapCommand(IEnumerable<IMovableMapObject> targets) : base(targets)
        {
        }

        public override void Apply()
        {
            int i = 0;
            var first = _before.Keys.FirstOrDefault();
            if (first != null)
            {
                var startX = first.X;
                var startY = first.Y;
                var startZ = first.Z;
                foreach (var kv in _before)
                {
                    kv.Key.X = DistributeOnX ? startX + i * X : kv.Value.X;
                    kv.Key.Y = DistributeOnY ? startY + i * Y : kv.Value.Y;
                    kv.Key.Z = DistributeOnZ ? startZ + i * Z : kv.Value.Z;
                    i++;
                }
            }
        }
    }
}
