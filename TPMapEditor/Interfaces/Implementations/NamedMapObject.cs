using TPMapEditor.Data;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Interfaces.Implementations
{
    public abstract partial class NamedMapObject : NamedObject, IMapObject
    {
        public WorldMap Map { get; }
        protected NamedMapObject(WorldMap map, string name) : base(name)
        {
            Map = map;
        }
    }
}
