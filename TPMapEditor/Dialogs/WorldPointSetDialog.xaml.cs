using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for WorldPointSetDialog.xaml
    /// </summary>
    public partial class WorldPointSetDialog : DialogWindow
    {

        [ObservableProperty]
        private WorldPointSet? selectedWorldPointSet;
        [ObservableProperty]
        private WorldPoint? selectedWorldPoint;
        public WorldMap Map { get; }

        public WorldPointSetDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWorldPointSet()
        {
            var worldPointSet = new WorldPointSet(Map, NamedElement.GenerateName("WorldPointSet", Map.WorldPointSets));
            worldPointSet.Points.Add(new(worldPointSet, 0, 0, 0, 0));
            Map.WorldPointSets.Add(worldPointSet);
        }

        [RelayCommand]
        private void OnAddWorldPoint()
        {
            SelectedWorldPointSet?.Points.Add(new(SelectedWorldPointSet, 0, 0, 0, 0));
        }

        private void RemoveWorldPointSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPointSet != null)
            {
                Map.WorldPointSets.Remove(SelectedWorldPointSet);
            }
        }

        private void RemoveWorldPointButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPoint != null)
            {
                SelectedWorldPointSet?.Points.Remove(SelectedWorldPoint);
            }
        }

        private void EditWorldPointSetColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPointSet != null)
            {
                var cp = new ColorPicker(this, SelectedWorldPointSet.Color) { Owner = this };
                if (cp.ShowDialog() == true)
                {
                    SelectedWorldPointSet.Color = cp.NewColor;
                }
            }
        }
    }
}
