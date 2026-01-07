using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel.SelectionTransform
{
    public partial class AlignTransformViewModel : SelectionTransformBaseViewModel
    {
        [ObservableProperty]
        private bool alignOnX, alignOnY, alignOnZ;
        [ObservableProperty]
        private double x, y, z;

        private readonly AlignTransformMapCommand internalCommand;
        protected override IUndoableMapCommand Command => internalCommand;

        public AlignTransformViewModel(IUndoManagerService undoManagerService, IEnumerable<IMovableMapObject> targets) : base(undoManagerService)
        {
            internalCommand = new(targets);
        }

        partial void OnXChanged(double value)
        {
            internalCommand.X = X;
            internalCommand.Apply();
        }

        partial void OnYChanged(double value)
        {
            internalCommand.Y = Y;
            internalCommand.Apply();
        }

        partial void OnZChanged(double value)
        {
            internalCommand.Z = Z;
            internalCommand.Apply();
        }

        partial void OnAlignOnXChanged(bool value)
        {
            internalCommand.AlignOnX = AlignOnX;
            internalCommand.Apply();
        }

        partial void OnAlignOnYChanged(bool value)
        {
            internalCommand.AlignOnY = AlignOnY;
            internalCommand.Apply();
        }

        partial void OnAlignOnZChanged(bool value)
        {
            internalCommand.AlignOnZ = AlignOnZ;
            internalCommand.Apply();
        }
    }
}
