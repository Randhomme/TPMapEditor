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

        public RelayCommand AddPlayerCommand { get; }

        public PlayerDialog(Window owner, WorldMap map, Action createPlayer) : base(owner)
        {
            Map = map;
            AddPlayerCommand = new(createPlayer);
            InitializeComponent();
            this.selectedPlayerX.Minimum = this.selectedPlayerY.Minimum =  -map.Size / 2 - 150;
            this.selectedPlayerX.Maximum = this.selectedPlayerY.Maximum = map.Size / 2 + 150;
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedPlayer != null)
            {
                SelectedPlayer.Remove?.Invoke();
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
