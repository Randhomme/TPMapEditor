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
    /// Interaction logic for SpeechEventDialog.xaml
    /// </summary>
    public partial class SpeechEventDialog : DialogWindow
    {
        [ObservableProperty]
        private SpeechEvent? selectedSpeechEvent;
        public WorldMap Map { get; }
        public SpeechEventDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddSpeechEvent()
        {
            Map.SpeechEvents.Add(new(Map, NamedElement.GenerateName("SpeechEvent", Map.SpeechEvents)));
        }

        private void RemoveSpeechEvent_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedSpeechEvent != null)
            {
                Map.SpeechEvents.Remove(SelectedSpeechEvent);
                SelectedSpeechEvent = null;
            }
        }

        private void EditTextColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpeechEvent != null)
            {
                var cp = new ColorPicker(this, SelectedSpeechEvent.TextColor) { Owner = this };
                if (cp.ShowDialog() == true)
                {
                    SelectedSpeechEvent.TextColor = cp.NewColor;
                }
            }
        }
    }
}
