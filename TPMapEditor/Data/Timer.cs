using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class Timer : NamedElement
    {
        [ObservableProperty]
        private bool status;
        [ObservableProperty]
        private float startTime;

        public Timer(string name, WorldMap map, bool status, float startTime) : base(map, name)
        {
            Status = status;
            StartTime = startTime;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.Timers)
            {
                if (item.Name == name && item!=this)
                    return true;
            }
            return false;
        }

    }
}
