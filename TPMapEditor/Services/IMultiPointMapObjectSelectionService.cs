using TPMapEditor.Interfaces;

namespace TPMapEditor.Services
{
    public interface IMultiPointMapObjectSelectionService<T1, T2> : ISelectionService<T1> where T1 : IMultiPointMapObject<T2> where T2 : ISelectableMapObject
    {
        public void SelectAndMakeLastSelectedWithoutPoints(T1 mapObject);

        public void RemoveFromSelectionWithoutPoints(T1 mapObject);
    }
}
