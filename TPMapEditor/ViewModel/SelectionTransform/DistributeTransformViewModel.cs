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
    public partial class DistributeTransformViewModel : SelectionTransformBaseViewModel
    {
        [ObservableProperty]
        private bool distributeOnX, distributeOnY, distributeOnZ;
        [ObservableProperty]
        private double x, y, z;

        private readonly DistributeTransformMapCommand internalCommand;
        protected override IUndoableMapCommand Command => internalCommand;

        public DistributeTransformViewModel(IUndoManagerService undoManagerService, IEnumerable<IMovableMapObject> targets) : base(undoManagerService)
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

        partial void OnDistributeOnXChanged(bool value)
        {
            internalCommand.DistributeOnX = DistributeOnX;
            internalCommand.Apply();
        }

        partial void OnDistributeOnYChanged(bool value)
        {
            internalCommand.DistributeOnY = DistributeOnY;
            internalCommand.Apply();
        }

        partial void OnDistributeOnZChanged(bool value)
        {
            internalCommand.DistributeOnZ = DistributeOnZ;
            internalCommand.Apply();
        }
    }
}
