using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public class EtheriumCurrent : NamedMapObject
    {
        public WaypointPath Path { get; set; }

        public EtheriumCurrent(WorldMap map, string name) : base(map, name)
        {
            Path = WaypointPath.DefaultWaypointPath;
        }

        protected override bool IsNameTaken(string name)
        {
            return false;
        }
    }
}
