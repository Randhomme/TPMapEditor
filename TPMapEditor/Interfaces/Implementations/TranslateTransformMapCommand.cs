using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces.Implementations
{
    public partial class TranslateTransformMapCommand : TransformMapCommand
    {
        [ObservableProperty]
        private double deltaX = 0, deltaY = 0, deltaZ = 0;

        private double deltaXbefore, deltaYbefore, deltaZbefore;
        private double deltaXafter, deltaYafter, deltaZafter;

        public bool Is3D { get; }

        public TranslateTransformMapCommand(IEnumerable<IMovableMapObject> targets, bool is3D) : base(targets)
        {
            deltaXbefore = deltaX;
            deltaYbefore = deltaY;
            deltaZbefore = deltaZ;
            Is3D = is3D;
        }

        public override void Undo()
        {
            deltaXafter = DeltaX;
            deltaYafter = DeltaY;
            deltaZafter = DeltaZ;
            (deltaXbefore, DeltaX) = (DeltaX, deltaXbefore);
            (deltaYbefore, DeltaY) = (DeltaY, deltaYbefore);
            (deltaZbefore, DeltaZ) = (DeltaZ, deltaZbefore);
            base.Undo();
        }

        public override void Redo()
        {
            base.Redo();
            deltaXbefore = DeltaX;
            deltaYbefore = DeltaY;
            deltaZbefore = DeltaZ;
            (deltaXafter, DeltaX) = (DeltaX, deltaXafter);
            (deltaYafter, DeltaY) = (DeltaY, deltaYafter);
            (deltaZafter, DeltaZ) = (DeltaZ, deltaZafter);
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
