using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Data;

namespace TPMapEditor.Settings
{
    /// <summary>
    /// A class to allow per TPGame folder game headers usage
    /// </summary>
    public class TPGameStringsSettings
    {
        public string TPGameFolderName { get; set; } = "TP_Game";
        public ObservableCollection<GameHeadersFile> TPTeamNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeechEvents { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeakerNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPShipNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPInGameMessages { get; } = new();
        public ObservableCollection<GameHeadersFile> TPJournalTitles { get; } = new();
        public ObservableCollection<GameHeadersFile> TPObjectiveTasks { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeechEventsJournals { get; } = new();
        public ObservableCollection<GameHeadersFile> TPMapTextItems { get; } = new();
        public ObservableCollection<GameHeadersFile> TPWorldNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPWorldDescriptions { get; } = new();

        public void AddDefaultHeadersToLists()
        {
            ClearHeadersLists();
            TPTeamNames.Add(new("TPTEAMNAMES_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS000_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS001_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS002_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS003_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS004_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS005_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTTUTORIAL_GameStrings.h"));
            TPSpeakerNames.Add(new("TPSPEAKERNAMES_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES00_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES01_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES02_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMENAVY00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMENAVY01_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPIRATE00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPIRATE01_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPROCYON00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPROCYON01_GameStrings.h"));
            TPInGameMessages.Add(new("TPINGAMEMESSAGE_GameStrings.h"));
            TPJournalTitles.Add(new("TPJOURNALSCREEN_GameStrings.h"));
            TPObjectiveTasks.Add(new("TPOBJECTIVES_GameStrings.h"));
            TPObjectiveTasks.Add(new("TPOBJECTIVES2_GameStrings.h"));
            TPSpeechEventsJournals.Add(new("TPSPEECHEVENTSJOURNALS_GameStrings.h"));
            TPMapTextItems.Add(new("TPMAPTEXTITEMS_GameStrings.h"));
            TPWorldNames.Add(new("TPWORLDNAMES_GameStrings.h"));
            TPWorldNames.Add(new("TPJOURNALSCREEN_GameStrings.h"));
            TPWorldDescriptions.Add(new("TPWORLDDESCRIPTION_GameStrings.h"));
        }

        public void ClearHeadersLists()
        {
            TPTeamNames.Clear();
            TPSpeechEvents.Clear();
            TPSpeakerNames.Clear();
            TPShipNames.Clear();
            TPInGameMessages.Clear();
            TPJournalTitles.Clear();
            TPObjectiveTasks.Clear();
            TPSpeechEventsJournals.Clear();
            TPMapTextItems.Clear();
            TPWorldNames.Clear();
            TPWorldDescriptions.Clear();
        }
    }
}
