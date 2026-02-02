using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class ObjectiveTask : SelectableNamedMapObject
    {
        [ObservableProperty]
        [property: Required]
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

        public override ICopiableMapObject Copy()
        {
            var copy = (ObjectiveTask)base.Copy();
            copy.Name = GenerateName($"{Name}_", Map.ObjectiveTasks);
            return copy;
        }
    }
}
