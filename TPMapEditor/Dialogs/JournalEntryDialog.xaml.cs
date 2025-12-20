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
        public WorldMap Map { get; }
        public Func<JournalEntry> Factory { get; }
        public JournalEntryDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            Map = map;
            Factory = () => new JournalEntry(StringDictionnary.SpeechEventsJournals.Keys.FirstOrDefault(), AppSettings.DialogueFilesList.FirstOrDefault(), AppSettings.HudTexturesList.FirstOrDefault());
            InitializeComponent();
        }
    }
}
