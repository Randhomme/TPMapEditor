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
    public partial class TranslateTransformViewModel : SelectionTransformBaseViewModel
    {
        [ObservableProperty]
        private double x, y, z;

        private readonly TranslateTransformMapCommand internalCommand;
        protected override IUndoableMapCommand Command => internalCommand;

        public TranslateTransformViewModel(IUndoManagerService undoManagerService, IEnumerable<IMovableMapObject> targets) : base(undoManagerService)
        {
            internalCommand = new(targets);
        }

        partial void OnXChanged(double value)
        {
            internalCommand.DeltaX = X;
            internalCommand.Apply();
        }

        partial void OnYChanged(double value)
        {
            internalCommand.DeltaY = Y;
            internalCommand.Apply();
        }

        partial void OnZChanged(double value)
        {
            internalCommand.DeltaZ = Z;
            internalCommand.Apply();
        }
    }
}
