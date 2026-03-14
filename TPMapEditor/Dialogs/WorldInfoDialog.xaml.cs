using System.Windows;
using System.Windows.Controls;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for WorldInfoDialog.xaml
    /// </summary>
    public partial class WorldInfoDialog : DialogWindow
    {
        public WorldMap Map { get; set; }
        public WorldInfoDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            this.Map = map;
            InitializeComponent();
            SetMapTypeComboBox();
        }

        private void MapTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (mapTypeComboBox.SelectedIndex)
            {
                case 0:
                    mustAssembleFleetCheckBox.IsChecked = isCampaignCheckBox.IsChecked = false;
                    mustAssembleFleetCheckBox.IsEnabled = isCampaignCheckBox.IsEnabled = false;
                    break;
                case 1:
                    mustAssembleFleetCheckBox.IsChecked = true;
                    isCampaignCheckBox.IsChecked = false;
                    mustAssembleFleetCheckBox.IsEnabled = isCampaignCheckBox.IsEnabled = false;
                    break;
                case 2:
                    mustAssembleFleetCheckBox.IsChecked = false;
                    isCampaignCheckBox.IsChecked = true;
                    mustAssembleFleetCheckBox.IsEnabled = isCampaignCheckBox.IsEnabled = false;
                    break;
                default:
                    mustAssembleFleetCheckBox.IsEnabled = isCampaignCheckBox.IsEnabled = true;
                    break;
            }
        }

        private void SetMapTypeComboBox()
        {
            if (mustAssembleFleetCheckBox.IsChecked == false)
            {
                if(isCampaignCheckBox.IsChecked == false)
                {
                    mapTypeComboBox.SelectedIndex = 0;
                }
                else
                {
                    mapTypeComboBox.SelectedIndex = 2;
                }

            }
            else if(isCampaignCheckBox.IsChecked == false)
            {
                mapTypeComboBox.SelectedIndex = 1;
            }
            else
            {
                mapTypeComboBox.SelectedIndex = 3;
            }
        }

        private void EditMapAmbientLightColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new ColorPicker(this, "Ambient light color", Map.AmbientLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.AmbientLightColor = cp.NewColor;
        }

        private void EditMapRoofLightColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new ColorPicker(this, "Roof light color", Map.RoofLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.RoofLightColor = cp.NewColor;
        }

        private void EditMapFloorLightColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new ColorPicker(this, "Floor light color", Map.FloorLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.FloorLightColor = cp.NewColor;
        }

        private void IsMultiplayerCheckBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!isMultiplayerCheckBox.IsEnabled)
                Map.IsMultiplayer = false; 
        }
    }
}
