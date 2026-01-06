using System.Collections.Generic;
using System.Collections.ObjectModel;
using TPMapEditor.Data;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract class MultiPointNamedMapObject<T> : SelectableNamedMapObject, IMultiPointMapObject<T> where T : ISelectableMapObject
    {
        private readonly ObservableCollection<T> points;
        public IList<T> Points { get => points; }

        protected MultiPointNamedMapObject(WorldMap map, string name) : base(map, name)
        {
            points = new ObservableCollection<T>();
            points.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Points));
            };
        }
    }
}
