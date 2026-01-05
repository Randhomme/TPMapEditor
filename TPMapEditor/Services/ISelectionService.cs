using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services
{
    public interface ISelectionService<T> where T : ISelectableMapObject
    {
        public IReadOnlyList<T> SelectedMapObjects { get; }
        public T? SelectedMapObject { get; }
        public void Select(T mapObject);
        public void AddToSelection(T mapObject);
        public void AddAllToSelection(IEnumerable<T> mapObjects);
        public void CtrlSelect(T mapObject);
        public void SelectAndMakeLastSelected(T mapObject);
        public void MakeLastSelected(T mapObject);
        public void Unselect(T mapObject);
        public void RemoveFromSelection(T mapObject);
        public void InvertSelection(IEnumerable<T> mapObjects);
        public void HideSelection();
        public void ShowAll(IEnumerable<T> mapObjects);
        public void ToggleSelectionVisibility();
        public void ClearSelection();
    }
}
