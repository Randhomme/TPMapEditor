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

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for WorldCrewAndArmsDialog.xaml
    /// </summary>
    public partial class WorldCrewAndArmsDialog : DialogWindow
    {
        [ObservableProperty]
        private WorldObjectType? selectedWorldCrew;
        [ObservableProperty]
        private WorldObjectType? selectedWorldArm;
        [ObservableProperty]
        private WorldObjectType selectedCrewType;
        [ObservableProperty]
        private WorldObjectType selectedArmType;
        public WorldMap Map { get; }
        public WorldCrewAndArmsDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            selectedCrewType = WorldObjectType.WotTypes.FirstOrDefault((t) => t.CustomInfoDefinition == Enums.WorldObjectDefinition.CustomInfoDefinition.CrewCustomInfoFactory);
            selectedArmType = WorldObjectType.WotTypes.FirstOrDefault((t) => t.CustomInfoDefinition == Enums.WorldObjectDefinition.CustomInfoDefinition.GunCustomInfoFactory);
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWorldCrew()
        {
            Map.WorldCrews.Add(SelectedCrewType);
        }

        [RelayCommand]
        private void OnAddWorldArm()
        {
            Map.WorldArms.Add(SelectedArmType);
        }

        private void RemoveWorldCrew_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldCrew != null)
                Map.WorldCrews.Remove(SelectedWorldCrew);
        }

        private void RemoveWorldArm_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldArm != null)
                Map.WorldArms.Remove(SelectedWorldArm);
        }
    }
}
