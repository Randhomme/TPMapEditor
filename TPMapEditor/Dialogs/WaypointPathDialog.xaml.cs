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
    /// Interaction logic for WaypointPathDialog.xaml
    /// </summary>
    public partial class WaypointPathDialog : DialogWindow
    {
        [ObservableProperty]
        private WaypointPath? selectedWaypointPath;
        [ObservableProperty]
        private WaypointPathPoint? selectedWaypointPathPoint;
        public WorldMap Map { get; }
        public WaypointPathDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWaypointPath()
        {
            var path = new WaypointPath(Map, NamedElement.GenerateName("Path", Map.WaypointPaths));
            path.Points.Add(new(path, 0, 0, 0));
            Map.WaypointPaths.Add(path);
        }

        [RelayCommand]
        private void OnAddWaypointPathPoint()
        {
            SelectedWaypointPath?.Points.Add(new(SelectedWaypointPath, 0, 0, 0));
        }

        private void RemoveWaypointPathButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedWaypointPath != null)
            {
                Map.WaypointPaths.Remove(SelectedWaypointPath);
            }
        }

        private void RemoveWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWaypointPathPoint != null)
            {
                SelectedWaypointPath?.Points.Remove(SelectedWaypointPathPoint);
            }
        }

        private void EditWaypointPathColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWaypointPath != null)
            {
                var cp = new ColorPicker(this, SelectedWaypointPath.Color) { Owner = this };
                if (cp.ShowDialog() == true)
                {
                    SelectedWaypointPath.Color = cp.NewColor;
                }
            }
        }
    }
}
