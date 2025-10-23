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
    /// Interaction logic for WorldInfoDialog.xaml
    /// </summary>
    public partial class WorldInfoDialog : DialogWindow
    {
        public WorldMap Map { get; set; }
        public WorldInfoDialog(Window owner, WorldMap map) : base(owner)
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
            var cp = new ColorPicker(this, Map.AmbientLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.AmbientLightColor = cp.NewColor;
        }

        private void EditMapRoofLightColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new ColorPicker(this, Map.RoofLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.RoofLightColor = cp.NewColor;
        }

        private void EditMapFloorLightColor_Click(object sender, RoutedEventArgs e)
        {
            var cp = new ColorPicker(this, Map.FloorLightColor, 255);
            if (cp.ShowDialog() == true)
                Map.FloorLightColor = cp.NewColor;
        }
    }
}
