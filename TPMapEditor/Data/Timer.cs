using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class Timer : NamedMapObject
    {
        [ObservableProperty]
        private bool status = false;
        [ObservableProperty]
        private double startTime = 0;

        public Timer(WorldMap map) : base(map, GenerateName("Timer", map.Timers))
        {
        }

        public Timer(WorldMap map, string name, bool status, float startTime) : base(map, name)
        {
            Status = status;
            StartTime = startTime;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.Timers)
            {
                if (item.Name == name && item!=this)
                    return true;
            }
            return false;
        }

    }
}
