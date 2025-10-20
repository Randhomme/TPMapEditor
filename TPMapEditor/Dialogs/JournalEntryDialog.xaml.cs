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
using TPMapEditor.Settings;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for JournalEntryDialog.xaml
    /// </summary>
    public partial class JournalEntryDialog : DialogWindow
    {
        [ObservableProperty]
        private JournalEntry? selectedJournalEntry;
        public WorldMap Map { get; }
        public JournalEntryDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddJournalEntry()
        {
            Map.JournalEntries.Add(new(Map, NamedElement.GenerateName("JournalEntry", Map.JournalEntries), StringDictionnary.SpeechEventsJournalsDictionnary.Keys.FirstOrDefault(), SpeechEvent.DialogueFilesList.FirstOrDefault(), AppSettings.GuiTextures.FirstOrDefault()));
        }

        private void RemoveJournalEntry_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJournalEntry != null)
            {
                Map.JournalEntries.Remove(SelectedJournalEntry);
                SelectedJournalEntry = null;
            }
        }
    }
}
