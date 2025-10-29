using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.Controls;
using TPMapEditor.Converter;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;
using TPMapEditor.Settings;

namespace TPMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        private Point moveActionPoint;
        private AppSettings settings;
        
        public WorldMap Map { get; }
        public WotGridItem? WotGridSelectedItem { get; set; }
        
        private void LoadSettings()
        {
            settings = settings.Load();
        }

        public MainWindow()
        {
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            settings = new AppSettings();
            LoadSettings();
            Map = new WorldMap();
            InitializeComponent();
            WotRadioButton.IsChecked = true;
        }

        #region MenuCommand

        [RelayCommand]
        private void OnMapImport()
        {
            var ofd = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = ".twt",
                Filter = "Map file (.twt)|*.twt",
                Title = "Select a map file",
            };
            if (ofd.ShowDialog(this) == true)
            {
                if(MessageBox.Show("The current map will be cleared. Continue ?", "Map import", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Map.Reset();
                    DataImport.ReadMapFileAndAddData(ofd.FileName, Map);
                }
            }
        }

        [RelayCommand]
        private void OnWorldInfoEdit()
        {
            new WorldInfoDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(this, Map.Size);
            if (msd.ShowDialog() == true)
            {
                Map.Size = msd.Size;
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new PlayerDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new WorldObjectDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnTeamsEdit()
        {
            if (StringDictionnary.TeamNames.Count <= 0)
                MessageBox.Show("You need to add at least one team name to create a team.");
            var td = new TeamsDialog(this, Map);
            td.ShowDialog();
        }

        [RelayCommand]
        private void OnGroupsEdit()
        {
            new GroupDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnFlagsEdit()
        {
            new FlagDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            if (Map.Players.Count > 1)
                new PlayerAllianceDialog(this, Map).ShowDialog();
            else
                MessageBox.Show("You need at least 2 players to create alliances.");
        }

        [RelayCommand]
        private void OnTimersEdit()
        {
            new TimerDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnSpeechEventsEdit()
        {
            new SpeechEventDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            new WorldRuleDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnObjectiveTasksEdit()
        {
            new ObjectiveTaskDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnJournalEntriesEdit()
        {
            new JournalEntryDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnAppSettingsEdit()
        {
            var asd = new AppSettingsDialog(this, settings);
            asd.ShowDialog();
        }

        #endregion MenuCommand

        #region MainWindow events

        //uncheck radio button on mouse down
        private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton!.IsChecked == true && e.ChangedButton == MouseButton.Left)
            {
                radioButton.IsChecked = false;
                e.Handled = true;
            }
        }

        //uncheck radio button when Enable changed
        private void RadioButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var rb = (RadioButton)sender;
            rb.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Unsaved changes will be lost. Exit now ?",
                "Exit?", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            settings.Save();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(settings.TpGamePath))
            {
                MessageBox.Show("You should set the TPGame path in the application settings before using the map editor.", "TPGame Path Not Set", MessageBoxButton.OK, MessageBoxImage.Warning);
                OnAppSettingsEdit();
            }
        }



        #endregion

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomSpeed = 0.1;
            double zoom = e.Delta > 0 ? 1 + zoomSpeed : 1 - zoomSpeed;

            ZoomTransform.ScaleX *= zoom;
            ZoomTransform.ScaleY *= zoom;
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Player clickedPlayer)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (!ctrlPressed)
                {
                    foreach (var p in Map.Players)
                        p.IsSelected = false;
                }

                clickedPlayer.IsSelected = !clickedPlayer.IsSelected;
                e.Handled = true;
            }
        }
    }
}
