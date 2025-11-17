using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
using TPMapEditor.Data;
using TPMapEditor.Settings;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for GameHeadersDialog.xaml
    /// </summary>
    public partial class GameStringsHeadersDialog : DialogWindow
    {
        [ObservableProperty]
        private GameHeadersFile? selectedTeamNamesHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedSpeechEventsHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedSpeakerNamesHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedShipNamesHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedInGameMessagesHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedObjectiveTasksHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedSpeechEventsJournalsHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedMapTextItemsHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedWorldNamesHeaderFile;
        [ObservableProperty]
        private GameHeadersFile? selectedWorldDescriptionsHeaderFile;
        public AppSettings AppSettings { get; }
        public GameStringsHeadersDialog(Window owner, AppSettings appSettings) : base(owner)
        {
            AppSettings = appSettings;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddTeamNamesHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPTeamNames.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddSpeechEventsHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPSpeechEvents.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddSpeakerNamesHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPSpeakerNames.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddShipNamesHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPShipNames.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddInGameMessagesHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPInGameMessages.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddObjectiveTasksHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPObjectiveTasks.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddSpeechEventsJournalsHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPSpeechEventsJournals.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddMapTextItemsHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPMapTextItems.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddWorldNamesHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPWorldNames.Add(new GameHeadersFile(fileName));
        }

        [RelayCommand]
        private void OnAddWorldDescriptionsHeaderFile()
        {
            string fileName = GameHeadersFile.GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
            AppSettings.TPWorldDescriptions.Add(new GameHeadersFile(fileName));
        }

        private void RemoveTeamNamesHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedTeamNamesHeaderFile != null)
            {
                AppSettings.TPTeamNames.Remove(SelectedTeamNamesHeaderFile);
                SelectedTeamNamesHeaderFile = null;
            }
        }

        private void RemoveSpeechEventsHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpeechEventsHeaderFile != null)
            {
                AppSettings.TPSpeechEvents.Remove(SelectedSpeechEventsHeaderFile);
                SelectedSpeechEventsHeaderFile = null;
            }
        }

        private void RemoveSpeakerNamesHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpeakerNamesHeaderFile != null)
            {
                AppSettings.TPSpeakerNames.Remove(SelectedSpeakerNamesHeaderFile);
                SelectedSpeakerNamesHeaderFile = null;
            }
        }

        private void RemoveShipNamesHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedShipNamesHeaderFile != null)
            {
                AppSettings.TPShipNames.Remove(SelectedShipNamesHeaderFile);
                SelectedShipNamesHeaderFile = null;
            }
        }

        private void RemoveInGameMessagesHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInGameMessagesHeaderFile != null)
            {
                AppSettings.TPInGameMessages.Remove(SelectedInGameMessagesHeaderFile);
                SelectedInGameMessagesHeaderFile = null;
            }
        }

        private void RemoveObjectiveTasksHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedObjectiveTasksHeaderFile != null)
            {
                AppSettings.TPObjectiveTasks.Remove(SelectedObjectiveTasksHeaderFile);
                SelectedObjectiveTasksHeaderFile = null;
            }
        }

        private void RemoveSpeechEventsJournalsHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpeechEventsJournalsHeaderFile != null)
            {
                AppSettings.TPSpeechEventsJournals.Remove(SelectedSpeechEventsJournalsHeaderFile);
                SelectedSpeechEventsJournalsHeaderFile = null;
            }
        }

        private void RemoveMapTextItemsHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMapTextItemsHeaderFile != null)
            {
                AppSettings.TPMapTextItems.Remove(SelectedMapTextItemsHeaderFile);
                SelectedMapTextItemsHeaderFile = null;
            }
        }

        private void RemoveWorldNamesHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldNamesHeaderFile != null)
            {
                AppSettings.TPWorldNames.Remove(SelectedWorldNamesHeaderFile);
                SelectedWorldNamesHeaderFile = null;
            }
        }

        private void RemoveWorldDescriptionsHeaderFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldDescriptionsHeaderFile != null)
            {
                AppSettings.TPWorldDescriptions.Remove(SelectedWorldDescriptionsHeaderFile);
                SelectedWorldDescriptionsHeaderFile = null;
            }
        }
    }
}
