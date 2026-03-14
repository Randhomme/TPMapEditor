using System;
using System.Linq;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class MultiPointMapObjectSelectionService<T1, T2> : SelectionService<T1>, IMultiPointMapObjectSelectionService<T1, T2> where T1 : IMultiPointMapObject<T2> where T2 : ISelectableMapObject
    {
        private readonly ISelectionService<T2> pointSelectionService;

        public MultiPointMapObjectSelectionService(ISelectionService<T2> pointSelectionService) : base()
        {
            this.pointSelectionService = pointSelectionService;
        }

        public override void Select(T1 mapObject)
        {
            base.Select(mapObject);
            foreach (var item in mapObject.Points)
            {
                pointSelectionService.AddToSelection(item);
            }
        }

        public override void Unselect(T1 mapObject)
        {
            base.Unselect(mapObject);
            foreach (var item in mapObject.Points)
            {
                pointSelectionService.RemoveFromSelection(item);
            }
        }

        public void SelectAndMakeLastSelectedWithoutPoints(T1 mapObject)
        {
            var selectedPoints = mapObject.Points.Where((p) => p.IsSelected).ToArray();
            AddToSelection(mapObject);
            foreach (var p in mapObject.Points)
            {
                if (!selectedPoints.Contains(p))
                    pointSelectionService.RemoveFromSelection(p);
            }
            MakeLastSelected(mapObject);
        }

        public void RemoveFromSelectionWithoutPoints(T1 mapObject)
        {
            if (!mapObject.Points.Any((p) => p.IsSelected))
                base.RemoveFromSelection(mapObject);
        }

        public override void ClearSelection()
        {
            foreach (var item in SelectedMapObjects)
            {
                foreach (var p in item.Points)
                {
                    pointSelectionService.RemoveFromSelection(p);
                }
            }
            base.ClearSelection();
        }
    }
}
