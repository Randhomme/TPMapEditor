using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
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
