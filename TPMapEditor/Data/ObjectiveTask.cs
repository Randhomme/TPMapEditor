using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class ObjectiveTask : NamedMapObject
    {
        [ObservableProperty]
        private string textStringId;
        [ObservableProperty]
        private bool active, completed, failed;

        public ObjectiveTask(WorldMap map, string name, string textStringId) : base(map, name)
        {
            this.textStringId = textStringId;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.ObjectiveTasks)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
