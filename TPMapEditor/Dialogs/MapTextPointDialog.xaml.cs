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
    /// Interaction logic for MapTextPointDialog.xaml
    /// </summary>
    public partial class MapTextPointDialog : DialogWindow
    {
        [ObservableProperty]
        private MapTextPoint? selectedMapTextPoint;
        public WorldMap Map { get; }

        public MapTextPointDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddMapTextPoint()
        {
            Map.MapTextPoints.Add(new(Map, NamedElement.GenerateName("MapTextPoint", Map.MapTextPoints), StringDictionnary.MapTextItems.Keys.FirstOrDefault()));
        }

        private void RemoveMapTextPoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMapTextPoint != null)
            {
                Map.MapTextPoints.Remove(SelectedMapTextPoint);
                SelectedMapTextPoint = null;
            }
        }
    }
}
