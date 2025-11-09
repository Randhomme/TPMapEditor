using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for WorldPolygonDialog.xaml
    /// </summary>
    public partial class WorldPolygonDialog : DialogWindow
    {
        [ObservableProperty]
        private WorldPolygon? selectedWorldPolygon;
        [ObservableProperty]
        private WorldPolygonPoint? selectedWorldPolygonPoint;
        public WorldMap Map { get; }

        public WorldPolygonDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWorldPolygon()
        {
            var polygon = new WorldPolygon(Map, NamedElement.GenerateName("WorldPolygon", Map.WorldPolygons));
            polygon.Points.Add(new(polygon, 0, 0));
            Map.WorldPolygons.Add(polygon);
        }

        [RelayCommand]
        private void OnAddWorldPolygonPoint()
        {
            SelectedWorldPolygon?.Points.Add(new(SelectedWorldPolygon, 0, 0));
        }

        private void RemoveWorldPolygonButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                Map.WorldPolygons.Remove(SelectedWorldPolygon);
            }
        }

        private void RemoveWorldPolygonPointButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPolygonPoint != null)
            {
                SelectedWorldPolygon?.Points.Remove(SelectedWorldPolygonPoint);
            }
        }

        private void EditWorldPolygonColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                var cp = new ColorPicker(this, SelectedWorldPolygon.Color) { Owner = this };
                if (cp.ShowDialog() == true)
                {
                    SelectedWorldPolygon.Color = cp.NewColor;
                }
            }
        }
    }
}
