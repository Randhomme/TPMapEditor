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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PlayerAllianceWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class PlayerAllianceDialog : DialogWindow
    {
        [ObservableProperty]
        private PlayerAlliance? selectedPlayerAlliance;
        public WorldMap Map { get; }
        public PlayerAllianceDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddPlayerAlliance()
        {
            Map.PlayerAlliances.Add(new PlayerAlliance(Map.Players[0], Map.Players[1]));
        }

        private void RemovePlayerAlliance_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlayerAlliance != null)
                Map.PlayerAlliances.Remove(SelectedPlayerAlliance);
        }
    }
}
