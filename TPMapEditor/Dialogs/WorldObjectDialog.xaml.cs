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
    /// Interaction logic for WorldObjectDialog.xaml
    /// </summary>
    public partial class WorldObjectDialog : DialogWindow
    {
        [ObservableProperty]
        private WorldObject? selectedWorldObject;
        public WorldMap Map { get; }
        public WorldObjectDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddWorldObject()
        {
            Map.WorldObjects.Add(new(WorldObjectType.WotTypes.FirstOrDefault(), 0, 0, 0));
        }

        private void RemoveWorldObject_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldObject != null)
            {
                Map.WorldObjects.Remove(SelectedWorldObject);
                SelectedWorldObject = null;
            }
        }
    }
}
