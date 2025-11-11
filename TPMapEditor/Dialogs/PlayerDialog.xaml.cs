using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TPMapEditor.Controls;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PlayerDialog.xaml
    /// </summary>
    public partial class PlayerDialog : DialogWindow
    {
        public WorldMap Map { get; }
        [ObservableProperty]
        private Player? selectedPlayer;

        public PlayerDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddPlayer()
        {
            Map.Players.Add(new(Map, NamedElement.GenerateName("Player", Map.Players), 0, 0, 0, 0, Colors.White));
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedPlayer != null)
            {
                Map.Players.Remove(SelectedPlayer);
                SelectedPlayer = null;
            }
        }

        private void EditPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlayer != null)
            {
                var cp = new ColorPicker(this, SelectedPlayer.Color) { Owner = this };
                if (cp.ShowDialog()==true)
                {
                    SelectedPlayer.Color = cp.NewColor;
                }
            }
        }
    }
}
