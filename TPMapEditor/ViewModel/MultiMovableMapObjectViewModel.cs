using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiMovableMapObjectViewModel<T> : MultiSelectableMapObjectViewModel<T>, IMultiMovableMapObject where T : IMovableMapObject
    {
        [ObservableProperty]
        private double x, y, z;

        private AlignTransformMapCommand? alignTransformMapCommand;

        public MultiMovableMapObjectViewModel(IEnumerable<T> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(T mapObject)
        {
            X = mapObject.X;
            Y = mapObject.Y;
            Z = mapObject.Z;
        }

        public void BeginAlignXTransformMapCommand(double multiXBefore)
        {
            if (UseUpdateCommands)
            {
                alignTransformMapCommand = new(this, multiXBefore, Y, Z, true)
                {
                    AlignOnX = true,
                    AlignOnY = false,
                    AlignOnZ = false,
                };
            }
        }

        public void BeginAlignYTransformMapCommand(double multiYBefore)
        {
            if (UseUpdateCommands)
            {
                alignTransformMapCommand = new(this, X, multiYBefore, Z, true)
                {
                    AlignOnX = false,
                    AlignOnY = true,
                    AlignOnZ = false,
                };
            }
        }

        public void BeginAlignZTransformMapCommand(double multiZBefore)
        {
            if (UseUpdateCommands)
            {
                alignTransformMapCommand = new(this, X, Y, multiZBefore, true)
                {
                    AlignOnX = false,
                    AlignOnY = false,
                    AlignOnZ = true,
                };
            }
        }

        public void UpdateAlignTransformMapCommand()
        {
            if (UseUpdateCommands)
            {
                alignTransformMapCommand!.X = X;
                alignTransformMapCommand!.Y = Y;
                alignTransformMapCommand!.Z = Z;
                alignTransformMapCommand.Apply();
            }
        }

        public void EndAlignTransformMapCommand()
        {
            if (UseUpdateCommands && alignTransformMapCommand != null)
            {
                undoManagerService.Push(alignTransformMapCommand);
                alignTransformMapCommand = null;
            }
        }

        public IEnumerable<IMovableMapObject> GetSelectedMovableMapObjects()
        {
            return selectedMapObjects.Cast<IMovableMapObject>();
        }
    }
}
