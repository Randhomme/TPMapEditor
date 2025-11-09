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
    /// Interaction logic for ObjectivePointDialog.xaml
    /// </summary>
    public partial class ObjectivePointDialog : DialogWindow
    {
        [ObservableProperty]
        private ObjectivePoint? selectedObjectivePoint;
        public WorldMap Map { get; }

        public ObjectivePointDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddObjectivePoint()
        {
            Map.ObjectivePoints.Add(new(Map, NamedElement.GenerateName("ObjectivePoint", Map.ObjectivePoints), 0, 0));
        }

        private void RemoveObjectivePoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedObjectivePoint != null)
            {
                Map.ObjectivePoints.Remove(SelectedObjectivePoint);
                SelectedObjectivePoint = null;
            }
        }
    }
}
