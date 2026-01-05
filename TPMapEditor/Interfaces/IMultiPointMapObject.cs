using System.Collections.Generic;

namespace TPMapEditor.Interfaces
{
    public interface IMultiPointMapObject<T> : ISelectableMapObject where T : ISelectableMapObject
    {
        public IList<T> Points { get; }
    }
}