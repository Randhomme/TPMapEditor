using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Utils.KeyboardShortcuts
{
    public class SelectableMapObjectKeyboardShortcutApplier<T> : KeyboardShortcutApplier where T : ISelectableMapObject
    {
        public IEnumerable<T> MapObjects { get; }
        public ICollection<T> SelectedObjects { get; }

        public SelectableMapObjectKeyboardShortcutApplier(IEnumerable<T> mapObjects, ICollection<T> selectedObjects)
        {
            MapObjects = mapObjects;
            SelectedObjects = selectedObjects;
        }

        /// <summary>
        /// Hide selected objects
        /// </summary>
        public override void OnHKey()
        {
            foreach (var item in SelectedObjects)
            {
                item.IsShownOnUi = false;
            }
        }

        /// <summary>
        /// Show hidden objects
        /// </summary>
        public override void OnShiftHKey()
        {
            foreach (var item in MapObjects)
            {
                item.IsShownOnUi = true;
            }
        }

        /// <summary>
        /// Toggle selected object visibility
        /// </summary>
        public override void OnCtrlHKey()
        {
            foreach (var item in SelectedObjects)
            {
                item.IsShownOnUi = !item.IsShownOnUi;
            }
        }

        /// <summary>
        /// Select all objects
        /// </summary>
        public override void OnAKey()
        {
            foreach (var item in MapObjects)
            {
                if (!item.IsSelected)
                    SelectedObjects.Add(item);
            }
        }

        /// <summary>
        /// Clear selection
        /// </summary>
        public override void OnShiftAKey()
        {
            foreach (var item in MapObjects)
            {
                if (item.IsSelected)
                    SelectedObjects.Remove(item);
            }
        }

        /// <summary>
        /// Invert selection
        /// </summary>
        public override void OnCtrlAKey()
        {
            foreach (var item in MapObjects)
            {
                item.IsSelected = !item.IsSelected;
                if (item.IsSelected)
                    SelectedObjects.Add(item);
                else
                    SelectedObjects.Remove(item);
            }
        }
    }
}
