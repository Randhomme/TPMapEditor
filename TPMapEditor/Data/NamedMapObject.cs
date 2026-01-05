using TPMapEditor.Interfaces;

namespace TPMapEditor.Data
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
