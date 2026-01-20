using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using TPMapEditor.Data.Rule;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class WorldRule : NamedMapObject
    {
        [ObservableProperty]
        private bool runOnce = true;
        public ObservableCollection<RuleCondition> Conditions { get; } = new ObservableCollection<RuleCondition>();
        public ObservableCollection<RuleAction> Actions { get; } = new ObservableCollection<RuleAction>();
        public Func<RuleCondition> RuleConditionFactory { get; }
        public Func<RuleAction> RuleActionFactory { get; }

        public WorldRule(WorldMap map, string name) : base(map, name)
        {
            RuleConditionFactory = () => new(Map);
            RuleActionFactory = () => new(Map);
            Actions.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach(RuleAction action in e.OldItems)
                    {
                        if (action.Type == Enums.RuleAction.StateInitSetupNebula && action.Nebula != null)
                            map.Nebulas.Remove(action.Nebula);
                        else if (action.Type == Enums.RuleAction.StateInitSetupShip && action.ShipUnit != null)
                            map.ShipUnits.Remove(action.ShipUnit);
                    }
                }
            };
        }

        protected override bool IsNameTaken(string name)
        {
            //foreach (var item in Map.WorldRules)
            //{
            //    if (item.Name == name && item != this)
            //        return true;
            //}
            return false;
        }
    }
}
