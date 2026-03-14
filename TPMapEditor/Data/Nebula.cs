using System.Linq;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public class Nebula : NamedMapObject
    {
        public WorldPolygon Polygon { get; set; }
        public WorldPointSet NebulaPointSet { get; set; }

        public Nebula(WorldMap map, string name) : base(map, name)
        {
            Polygon = map.WorldPolygons.FirstOrDefault();
            NebulaPointSet = WorldPointSet.DefaultWorldPointSet;
        }

        protected override bool IsNameTaken(string name)
        {
            return false;
        }
    }
}
