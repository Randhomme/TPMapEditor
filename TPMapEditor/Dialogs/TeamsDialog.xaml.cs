using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TeamsDialog.xaml
    /// </summary>
    public partial class TeamsDialog : DialogWindow
    {
        public Team? SelectedSelectableTeam { get; set; }
        public Team? SelectedInGameTeam { get; set; }
        public WorldMap Map { get; }
        public TeamsDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddSelectableTeam()
        {
            Map.SelectableTeams.Add(new Team(StringDictionnary.TeamNames.Keys.FirstOrDefault()) { Race = Enums.Race.Navy });
        }

        private void RemoveSelectedSelectableTeamButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSelectableTeam != null)
            {
                foreach (var player in Map.Players)
                {
                    if (player.SelectableTeam == SelectedSelectableTeam)
                    {
                        player.SelectableTeam = null;
                    }
                }
                Map.InGameTeams.Remove(SelectedSelectableTeam);
                SelectedSelectableTeam = null;
            }
        }

        [RelayCommand]
        private void OnAddInGameTeam()
        {
            Map.InGameTeams.Add(new Team(StringDictionnary.TeamNames.Keys.FirstOrDefault()) { Race = Enums.Race.Navy });
        }

        private void RemoveSelectedInGameTeamButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInGameTeam != null)
            {
                foreach (var player in Map.Players)
                {
                    if (player.InGameTeam == SelectedInGameTeam)
                    {
                        player.InGameTeam = null;
                    }
                }
                Map.InGameTeams.Remove(SelectedInGameTeam);
                SelectedInGameTeam = null;
            }
        }
    }
}
