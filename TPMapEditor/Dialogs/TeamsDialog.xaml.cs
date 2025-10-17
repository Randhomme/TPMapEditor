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
        public Team? SelectedTeam { get; set; }
        public WorldMap Map { get; }
        public TeamsDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        private bool CanAddTeam()
        {
            return Map.Teams.Count < 8 && Team.TeamNames.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanAddTeam))]
        private void OnAddTeam()
        {
            Map.Teams.Add(new Team(Team.TeamNames.Keys.First()) { Race = Enums.Race.Navy });
            AddTeamCommand.NotifyCanExecuteChanged();
        }

        //this can't be a command because the button's context is a team (in the datagrid)
        private void RemoveTeam()
        {
            if (SelectedTeam != null)
            {
                Map.Teams.Remove(SelectedTeam);
                foreach(var player in Map.Players)
                {
                    if (player.Team == SelectedTeam)
                    {
                        player.Team = null;
                    }
                }
                AddTeamCommand.NotifyCanExecuteChanged();
                SelectedTeam = null;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveTeam();
        }
    }
}
