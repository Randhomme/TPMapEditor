using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces.Implementations
{
    public class TranslateTransformMapCommand : TransformMapCommand
    {
        public double DeltaX { get; set; } = 0;
        public double DeltaY { get; set; } = 0;
        public double DeltaZ { get; set; } = 0;

        public TranslateTransformMapCommand(IEnumerable<IMovableMapObject> targets) : base(targets)
        {
        }

        public override void Apply()
        {
            foreach (var kv in _before)
            {
                kv.Key.X = kv.Value.X + DeltaX;
                kv.Key.Y = kv.Value.Y + DeltaY;
                kv.Key.Z = kv.Value.Z + DeltaZ;
            }
        }
    }
}
