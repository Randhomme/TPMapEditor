using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TPMapEditor.Data;
using TPMapEditor.Data.Rule;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for WorldRuleDialog.xaml
    /// </summary>
    public partial class WorldRuleDialog : DialogWindow
    {
        public WorldMap Map { get; }
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveUpWorldRuleCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveDownWorldRuleCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddRuleConditionCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddRuleActionCommand))]
        private WorldRule? selectedWorldRule;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveUpRuleConditionCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveDownRuleConditionCommand))]
        private RuleCondition? selectedRuleCondition;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveUpRuleActionCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveDownRuleActionCommand))]
        private RuleAction? selectedRuleAction;

        public WorldRuleDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWorldRule()
        {
            Map.WorldRules.Add(new(Map, NamedElement.GenerateName("Rule", Map.WorldRules)));
            MoveUpWorldRuleCommand.NotifyCanExecuteChanged();
            MoveDownWorldRuleCommand.NotifyCanExecuteChanged();
        }

        private bool IsSelectedWorldRuleNull() => SelectedWorldRule != null;

        private bool CanMoveUpWorldRule()
        {
            return SelectedWorldRule != null && Map.WorldRules.IndexOf(SelectedWorldRule) > 0;
        }

        private bool CanMoveDownWorldRule()
        {
            return SelectedWorldRule != null && Map.WorldRules.IndexOf(SelectedWorldRule) < Map.WorldRules.Count - 1;
        }

        [RelayCommand(CanExecute = nameof(CanMoveUpWorldRule))]
        private void OnMoveUpWorldRule()
        {
            var index = Map.WorldRules.IndexOf(SelectedWorldRule!);
            Map.WorldRules.Move(index, index - 1);
            MoveUpWorldRuleCommand.NotifyCanExecuteChanged();
            MoveDownWorldRuleCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveDownWorldRule))]
        private void OnMoveDownWorldRule()
        {
            var index = Map.WorldRules.IndexOf(SelectedWorldRule!);
            Map.WorldRules.Move(index, index + 1);
            MoveUpWorldRuleCommand.NotifyCanExecuteChanged();
            MoveDownWorldRuleCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(IsSelectedWorldRuleNull))]
        private void OnAddRuleCondition()
        {
            SelectedWorldRule?.Conditions.Add(new(Map));
            MoveUpRuleConditionCommand.NotifyCanExecuteChanged();
            MoveDownRuleConditionCommand.NotifyCanExecuteChanged();
        }

        private bool CanMoveUpRuleCondition()
        {
            return SelectedRuleCondition != null && SelectedWorldRule?.Conditions.IndexOf(SelectedRuleCondition) > 0;
        }

        private bool CanMoveDownRuleCondition()
        {
            return SelectedRuleCondition != null && SelectedWorldRule?.Conditions.IndexOf(SelectedRuleCondition) < SelectedWorldRule?.Conditions.Count - 1;
        }

        [RelayCommand(CanExecute = nameof(CanMoveUpRuleCondition))]
        private void OnMoveUpRuleCondition()
        {
            var index = SelectedWorldRule!.Conditions.IndexOf(SelectedRuleCondition!);
            SelectedWorldRule.Conditions.Move(index, index - 1);
            MoveUpRuleConditionCommand.NotifyCanExecuteChanged();
            MoveDownRuleConditionCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveDownRuleCondition))]
        private void OnMoveDownRuleCondition()
        {
            var index = SelectedWorldRule!.Conditions.IndexOf(SelectedRuleCondition!);
            SelectedWorldRule.Conditions.Move(index, index + 1);
            MoveUpRuleConditionCommand.NotifyCanExecuteChanged();
            MoveDownRuleConditionCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(IsSelectedWorldRuleNull))]
        private void OnAddRuleAction()
        {
            SelectedWorldRule?.Actions.Add(new(Map));
            MoveUpRuleActionCommand.NotifyCanExecuteChanged();
            MoveDownRuleActionCommand.NotifyCanExecuteChanged();
        }

        private bool CanMoveUpRuleAction()
        {
            return SelectedRuleAction != null && SelectedWorldRule?.Actions.IndexOf(SelectedRuleAction) > 0;
        }

        private bool CanMoveDownRuleAction()
        {
            return SelectedRuleAction != null && SelectedWorldRule?.Actions.IndexOf(SelectedRuleAction) < SelectedWorldRule?.Actions.Count - 1;
        }

        [RelayCommand(CanExecute = nameof(CanMoveUpRuleAction))]
        private void OnMoveUpRuleAction()
        {
            var index = SelectedWorldRule!.Actions.IndexOf(SelectedRuleAction!);
            SelectedWorldRule.Actions.Move(index, index - 1);
            MoveUpRuleActionCommand.NotifyCanExecuteChanged();
            MoveDownRuleActionCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveDownRuleAction))]
        private void OnMoveDownRuleAction()
        {
            var index = SelectedWorldRule!.Actions.IndexOf(SelectedRuleAction!);
            SelectedWorldRule.Actions.Move(index, index + 1);
            MoveUpRuleActionCommand.NotifyCanExecuteChanged();
            MoveDownRuleActionCommand.NotifyCanExecuteChanged();
        }

        private void RemoveWorldRule_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldRule != null)
            {
                Map.WorldRules.Remove(SelectedWorldRule);
                SelectedWorldRule = null;
            }
        }

        private void RemoveRuleCondition_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldRule != null && SelectedRuleCondition != null)
            {
                SelectedWorldRule.Conditions.Remove(SelectedRuleCondition);
                SelectedRuleCondition = null;
            }
        }

        private void RemoveRuleAction_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldRule != null && SelectedRuleAction != null)
            {
                if (SelectedRuleAction.Type == Enums.RuleAction.StateInitSetupShip && SelectedRuleAction.ShipUnit != null)
                {
                    Map.ShipUnits.Remove(SelectedRuleAction.ShipUnit);
                    SelectedRuleAction.ShipUnit = null;
                }
                SelectedWorldRule.Actions.Remove(SelectedRuleAction);
                SelectedRuleAction = null;
            }
        }

        private void GroupUnitComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = (ComboBox)sender;
            if (combo.SelectedIndex == -1 && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }
    }
}
