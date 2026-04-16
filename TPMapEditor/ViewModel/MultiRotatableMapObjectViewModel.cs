using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiRotatableMapObjectViewModel<T> : MultiMovableMapObjectViewModel<T>, IMultiRotatableMapObject where T : IRotatableMapObject
    {
        [ObservableProperty]
        private double xRotation, yRotation, zRotation;

        private RotateSpinFixTransformMapCommand? rotateSpinFixTransformMapCommand;

        public MultiRotatableMapObjectViewModel(IEnumerable<T> selectedMapObjects, IUndoManagerService undoManagerService) : base(selectedMapObjects, undoManagerService)
        {
        }

        protected override void UpdateFromMapObject_Internal(T mapObject)
        {
            base.UpdateFromMapObject_Internal(mapObject);
            XRotation = mapObject.XRotation;
            YRotation = mapObject.YRotation;
            ZRotation = mapObject.ZRotation;
        }

        public void BeginSpinXFixTransformMapCommand()
        {
            if (UseUpdateCommands)
            {
                rotateSpinFixTransformMapCommand = new(this)
                {
                    RotateOnX = true,
                    RotateOnY = false,
                    RotateOnZ = false,
                };
            }
        }

        public void BeginSpinYFixTransformMapCommand()
        {
            if (UseUpdateCommands)
            {
                rotateSpinFixTransformMapCommand = new(this)
                {
                    RotateOnX = false,
                    RotateOnY = true,
                    RotateOnZ = false,
                };
            }
        }

        public void BeginSpinZFixTransformMapCommand()
        {
            if (UseUpdateCommands)
            {
                rotateSpinFixTransformMapCommand = new(this)
                {
                    RotateOnX = false,
                    RotateOnY = false,
                    RotateOnZ = true,
                };
            }
        }

        public void UpdateSpinFixTransformMapCommand()
        {
            if (UseUpdateCommands)
            {
                rotateSpinFixTransformMapCommand!.XRotation = XRotation;
                rotateSpinFixTransformMapCommand!.YRotation = YRotation;
                rotateSpinFixTransformMapCommand!.ZRotation = ZRotation;
            }
        }

        public void EndSpinFixTransformMapCommand()
        {
            if (UseUpdateCommands && rotateSpinFixTransformMapCommand != null)
            {
                undoManagerService.Push(rotateSpinFixTransformMapCommand);
                rotateSpinFixTransformMapCommand = null;
            }
        }

        public IEnumerable<IRotatableMapObject> GetSelectedRotatableMapObjects()
        {
            return selectedMapObjects.Cast<IRotatableMapObject>();
        }
    }
}
