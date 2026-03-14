using TPMapEditor.Data;
using TPMapEditor.Utils;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract partial class MapObject : CustomObservableValidator, IMapObject
    {
        public WorldMap Map { get; }
        protected MapObject(WorldMap map)
        {
            Map = map;
        }
    }
}
