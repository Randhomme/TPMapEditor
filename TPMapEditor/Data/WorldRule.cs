using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using TPMapEditor.Data.Rule;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.Data
{
    public partial class WorldRule : SelectableNamedMapObject
    {
        private readonly ICopyPasteService copyPasteService;
        private bool canPasteConditions = false;
        private bool canPasteActions = false;
        private readonly ObservableCollection<RuleCondition> selectedConditions = new();
        private readonly ObservableCollection<RuleAction> selectedActions = new();
        [ObservableProperty]
        private bool runOnce = true;
        public ObservableCollection<RuleCondition> Conditions { get; } = new ObservableCollection<RuleCondition>();
        public ObservableCollection<RuleAction> Actions { get; } = new ObservableCollection<RuleAction>();
        public Func<RuleCondition> RuleConditionFactory { get; }
        public Func<RuleAction> RuleActionFactory { get; }
        public ICollection<RuleCondition> SelectedConditions { get => selectedConditions; }
        public ICollection<RuleAction> SelectedActions { get => selectedActions; }

        public WorldRule(WorldMap map, string name, ICopyPasteService copyPasteService) : base(map, name)
        {
            this.copyPasteService = copyPasteService;
            RuleConditionFactory = () => new(Map);
            RuleActionFactory = () => new(Map);
            Actions.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach(RuleAction action in e.OldItems)
                    {
                        if (action.Type == Enums.RuleAction.StateInitSetupEtheriumCurrent && action.EtheriumCurrent != null)
                            map.EtheriumCurrents.Remove(action.EtheriumCurrent);
                        else if (action.Type == Enums.RuleAction.StateInitSetupNebula && action.Nebula != null)
                            map.Nebulas.Remove(action.Nebula);
                        else if (action.Type == Enums.RuleAction.StateInitSetupShip && action.ShipUnit != null)
                            map.ShipUnits.Remove(action.ShipUnit);
                    }
                }
            };
            selectedConditions.CollectionChanged += (s, e) =>
            {
                CopyConditionsCommand.NotifyCanExecuteChanged();
            };
            selectedActions.CollectionChanged += (s, e) =>
            {
                CopyActionsCommand.NotifyCanExecuteChanged();
            };
            copyPasteService.ClearClipboard();
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

        public override ICopiableMapObject Copy()
        {
            var copy = new WorldRule(Map, GenerateName($"{Name}_", Map.WorldRules), copyPasteService);
            foreach (var item in Conditions)
            {
                var condition = new RuleCondition(Map) { Type = item.Type };
                condition.RuleFields.Clear();
                foreach (var rf in item.RuleFields)
                {
                    condition.RuleFields.Add((RuleField)rf.Copy());
                }
                copy.Conditions.Add(condition);
            }
            foreach (var item in Actions)
            {
                var action = new RuleAction(Map) { Type = item.Type };
                action.RuleFields.Clear();
                foreach (var rf in item.RuleFields)
                {
                    action.RuleFields.Add((RuleField)rf.Copy());
                }
                copy.Actions.Add(action);
            }
            return copy;
        }

        //TODO : Move everything below into a ViewModel, it needs a solid amount of refactoring

        [RelayCommand(CanExecute = nameof(CanCopyConditions))]
        private void OnCopyConditions()
        {
            copyPasteService.Copy(SelectedConditions);
            canPasteConditions = true;
            PasteConditionsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPasteConditions))]
        private void OnPasteConditions()
        {
            var pastedItems = copyPasteService.Paste<RuleCondition>();
            foreach (var item in pastedItems)
            {
                Conditions.Add(item);
            }
        }

        [RelayCommand(CanExecute = nameof(CanCopyActions))]
        private void OnCopyActions()
        {
            copyPasteService.Copy(SelectedActions);
            canPasteActions = true;
            PasteActionsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPasteActions))]
        private void OnPasteActions()
        {
            var pastedItems = copyPasteService.Paste<RuleAction>();
            foreach (var item in pastedItems)
            {
                Actions.Add(item);
            }
        }

        private bool CanCopyConditions() => SelectedConditions.Count > 0;

        private bool CanPasteConditions() => canPasteConditions;

        private bool CanCopyActions() => SelectedActions.Count > 0;

        private bool CanPasteActions() => canPasteActions;
    }
}
