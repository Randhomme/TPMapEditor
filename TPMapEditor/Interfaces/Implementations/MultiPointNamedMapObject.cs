using System.Collections.Generic;
using System.Collections.ObjectModel;
using TPMapEditor.Data;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract class MultiPointNamedMapObject<T> : SelectableNamedMapObject, IMultiPointMapObject<T> where T : ISelectableMapObject
    {
        private ObservableCollection<T> points;
        public IList<T> Points { get => points; }

        protected MultiPointNamedMapObject(WorldMap map, string name) : base(map, name)
        {
            points = new ObservableCollection<T>();
            points.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Points));
            };
        }

        //public override ISelectableMapObject Copy()
        //{
        //    var copy = (MultiPointNamedMapObject<T>)this.MemberwiseClone();
        //    points = new ObservableCollection<T>();
        //    for (int i = 0; i < copy.Points.Count; i++)
        //    {
        //        points.Add((T)copy.Points[i].Copy());
        //    }
        //    return copy;
        //}
    }
}
