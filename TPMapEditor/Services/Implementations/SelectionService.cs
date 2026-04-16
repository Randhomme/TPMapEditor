using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    /// <summary>
    /// Generic implementation of ISelectionService
    /// </summary>
    /// <typeparam name="T">The type of object we want to select</typeparam>
    public partial class SelectionService<T> : ObservableObject, ISelectionService<T> where T : ISelectableMapObject
    {
        private T? selectedMapObject;
        private readonly ObservableCollection<T> selectedMapObjects = new();

        public T? SelectedMapObject 
        {
            get => selectedMapObject;
            private set
            {
                SetProperty(ref selectedMapObject, value);
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IReadOnlyList<T> SelectedMapObjects { get => selectedMapObjects; }

        public EventHandler? SelectionChanged { get; set; }

        public SelectionService()
        {
            selectedMapObjects.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (T item in e.NewItems)
                    {
                        Select(item);
                    }
                    var last = SelectedMapObjects.LastOrDefault();
                    MakeLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (T item in e.OldItems)
                    {
                        Unselect(item);
                    }
                    var last = SelectedMapObjects.LastOrDefault();
                    MakeLastSelected(last);
                }
                SelectionChanged?.Invoke(s, e);
            };
        }

        public virtual void Select(T mapObject)
        {
            mapObject.IsSelected = true;
        }

        public virtual void AddToSelection(T mapObject)
        {
            if (!mapObject.IsSelected)
            {
                selectedMapObjects.Add(mapObject);
            }
        }

        public virtual void AddAllToSelection(IEnumerable<T> mapObjects)
        {
            foreach (var item in mapObjects)
            {
                AddToSelection(item);
            }
        }

        public virtual void CtrlSelect(T mapObject)
        {
            if (mapObject.IsLastSelected)
            {
                RemoveFromSelection(mapObject);
            }
            else
            {
                SelectAndMakeLastSelected(mapObject);
            }
        }

        public virtual void SelectAndMakeLastSelected(T mapObject)
        {
            AddToSelection(mapObject);
            MakeLastSelected(mapObject);
        }

        public virtual void MakeLastSelected(T mapObject)
        {
            if (SelectedMapObject != null)
            {
                SelectedMapObject.IsLastSelected = false;
            }
            SelectedMapObject = mapObject;
            if (SelectedMapObject != null)
            {
                SelectedMapObject.IsLastSelected = true;
            }
        }

        public virtual void Unselect(T mapObject)
        {
            mapObject.IsSelected = mapObject.IsLastSelected = false;
        }

        public virtual void RemoveFromSelection(T mapObject)
        {
            if (mapObject.IsSelected)
            {
                selectedMapObjects.Remove(mapObject);
            }
        }

        public virtual void InvertSelection(IEnumerable<T> mapObjects)
        {
            foreach (var item in mapObjects)
            {
                item.IsSelected = !item.IsSelected;
                if (item.IsSelected)
                    selectedMapObjects.Add(item);
                else
                    selectedMapObjects.Remove(item);
            }
        }

        public virtual void HideSelection()
        {
            foreach (var item in SelectedMapObjects)
            {
                item.IsShownOnUi = false;
            }
        }

        public virtual void ShowAll(IEnumerable<T> mapObjects)
        {
            foreach (var item in mapObjects)
            {
                item.IsShownOnUi = true;
            }
        }

        public virtual void ToggleSelectionVisibility()
        {
            foreach (var item in SelectedMapObjects)
            {
                item.IsShownOnUi = !item.IsShownOnUi;
            }
        }

        public virtual void ClearSelection()
        {
            foreach (var item in SelectedMapObjects)
            {
                item.IsSelected = item.IsLastSelected = false;
            }
            selectedMapObjects.Clear();
            SelectedMapObject = default;
        }
    }
}
