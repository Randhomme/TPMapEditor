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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TimerDialog.xaml
    /// </summary>
    [ObservableObject]
    public partial class TimerDialog : DialogWindow
    {
        [ObservableProperty]
        private Timer? selectedTimer;
        public WorldMap Map { get; }
        public TimerDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddTimer()
        {
            Map.Timers.Add(new Timer(NamedElement.GenerateName("Timer", Map.Timers), Map, false, 0));
        }

        private void RemoveTimer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTimer != null)
                Map.Timers.Remove(SelectedTimer);
        }
    }
}
