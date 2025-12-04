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
        [NotifyCanExecuteChangedFor(nameof(AddRuleConditionCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddRuleActionCommand))]
        private WorldRule? selectedWorldRule;
        [ObservableProperty]
        private RuleCondition? selectedRuleCondition;
        [ObservableProperty]
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
        }

        private bool IsSelectedWorldRuleNull() => SelectedWorldRule != null;

        [RelayCommand(CanExecute = nameof(IsSelectedWorldRuleNull))]
        private void OnAddRuleCondition()
        {
            SelectedWorldRule?.Conditions.Add(new(Map));
        }

        [RelayCommand(CanExecute = nameof(IsSelectedWorldRuleNull))]
        private void OnAddRuleAction()
        {
            SelectedWorldRule?.Actions.Add(new(Map));
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
