using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Data.Rule;

namespace TPMapEditor.Data
{
    public partial class WorldRule : NamedElement
    {
        [ObservableProperty]
        private bool runOnce = true;
        public ObservableCollection<RuleCondition> Conditions { get; } = new ObservableCollection<RuleCondition>();
        public ObservableCollection<RuleAction> Actions { get; } = new ObservableCollection<RuleAction>();

        public WorldRule(WorldMap map, string name) : base(map, name)
        {
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WorldRules)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
