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
    /// Interaction logic for ObjectiveTaskDialog.xaml
    /// </summary>
    public partial class ObjectiveTaskDialog : DialogWindow
    {
        public WorldMap Map { get; }
        [ObservableProperty]
        private ObjectiveTask? selectedObjectiveTask;
        public ObjectiveTaskDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddObjectiveTask()
        {
            var objectiveTask = new ObjectiveTask(Map, NamedElement.GenerateName("ObjectiveTask", Map.ObjectiveTasks), StringDictionnary.ObjectiveTasksDictionnary.Keys.FirstOrDefault());
            Map.ObjectiveTasks.Add(objectiveTask);
        }

        private void RemoveObjectiveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedObjectiveTask != null)
            {
                Map.ObjectiveTasks.Remove(SelectedObjectiveTask);
                SelectedObjectiveTask = null;
            }
        }
    }
}
