using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;
using TPMapEditor.Enums.WorldObjectDefinition;
using TPMapEditor.Interfaces;
using TPMapEditor.Settings;
using TPMapEditor.Utils.KeyboardShortcuts;

namespace TPMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        private Point moveActionPoint;
        private DateTime lastWheelTime = DateTime.MinValue;
        private AppSettings settings;
        private readonly SelectableMapObjectKeyboardShortcutApplier<WorldObject> worldObjectKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<Player> playerKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<WaypointPath> waypointPathKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<WorldPolygon> worldPolygonKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<WorldPointSet> worldPointSetKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<ObjectivePoint> objectivePointKeyboardShortcutApplier;
        private readonly SelectableMapObjectKeyboardShortcutApplier<MapTextPoint> mapTextPointKeyboardShortcutApplier;
        [ObservableProperty]
        private WorldObjectType? selectedWorldObjectType;
        [ObservableProperty]
        private WorldObject? selectedWorldObject;
        [ObservableProperty]
        private Player? selectedPlayer;
        [ObservableProperty]
        private WaypointPathPoint? selectedWaypointPathPoint;
        [ObservableProperty]
        private WaypointPath? selectedWaypointPath;
        [ObservableProperty]
        private WorldPolygonPoint? selectedWorldPolygonPoint;
        [ObservableProperty]
        private WorldPolygon? selectedWorldPolygon;
        [ObservableProperty]
        private WorldPoint? selectedWorldPoint;
        [ObservableProperty]
        private WorldPointSet? selectedWorldPointSet;
        [ObservableProperty]
        private ObjectivePoint? selectedObjectivePoint;
        [ObservableProperty]
        private MapTextPoint? selectedMapTextPoint;
        [ObservableProperty]
        private UIElement? selectedElement;

        public ObservableCollection<WorldObject> SelectedWorldObjects { get; } = new();
        public ObservableCollection<Player> SelectedPlayers { get; } = new();
        public ObservableCollection<WaypointPathPoint> SelectedWaypointPathPoints { get; } = new();
        public ObservableCollection<WaypointPath> SelectedWaypointPaths { get; } = new();
        public ObservableCollection<WorldPolygonPoint> SelectedWorldPolygonPoints { get; } = new();
        public ObservableCollection<WorldPolygon> SelectedWorldPolygons { get; } = new();
        public ObservableCollection<WorldPoint> SelectedWorldPoints { get; } = new();
        public ObservableCollection<WorldPointSet> SelectedWorldPointSets { get; } = new();
        public ObservableCollection<ObjectivePoint> SelectedObjectivePoints { get; } = new();
        public ObservableCollection<MapTextPoint> SelectedMapTextPoints { get; } = new();
        public ICollectionView SelectableWorldObjectTypes { get; }

        public WorldMap Map { get; private set; }

        public MainWindow()
        {
            SelectableWorldObjectTypes = new CollectionViewSource() { Source = WorldObjectType.WotTypes }.View;
            SelectableWorldObjectTypes.Filter = IsSelectableWorldObjectType;
            settings = new AppSettings();
            Map = new WorldMap();
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"{Title} v{version.Major}.{version.Minor}.{version.Build}";
            WorldObjectRadioButton.IsChecked = true;
            HidePlayerElements();
            HideWaypointPathElements();
            HideWorldPolygonElements();
            HideWorldPointSetElements();
            HideObjectivePointElements();
            HideMapTextPointElements();
            worldObjectKeyboardShortcutApplier = new(Map.WorldObjects, SelectedWorldObjects);
            playerKeyboardShortcutApplier = new(Map.Players, SelectedPlayers);
            waypointPathKeyboardShortcutApplier = new(Map.WaypointPaths, SelectedWaypointPaths);
            worldPolygonKeyboardShortcutApplier = new(Map.WorldPolygons, SelectedWorldPolygons);
            worldPointSetKeyboardShortcutApplier = new(Map.WorldPointSets, SelectedWorldPointSets);
            objectivePointKeyboardShortcutApplier = new(Map.ObjectivePoints, SelectedObjectivePoints);
            mapTextPointKeyboardShortcutApplier = new(Map.MapTextPoints, SelectedMapTextPoints);

            SelectedWorldObjects.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (WorldObject wot in e.NewItems)
                    {
                        SelectWorldObject(wot);
                    }
                    var last = SelectedWorldObjects.LastOrDefault();
                    MakeWorldObjectLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (WorldObject wot in e.OldItems)
                    {
                        UnselectWorldObject(wot);
                    }
                    var last = SelectedWorldObjects.LastOrDefault();
                    MakeWorldObjectLastSelected(last);
                }
            };
            SelectedPlayers.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Player obj in e.NewItems)
                    {
                        SelectPlayer(obj);
                    }
                    var last = SelectedPlayers.LastOrDefault();
                    MakePlayerLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Player obj in e.OldItems)
                    {
                        UnselectPlayer(obj);
                    }
                    var last = SelectedPlayers.LastOrDefault();
                    MakePlayerLastSelected(last);
                }
            };
            SelectedWaypointPaths.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (WaypointPath obj in e.NewItems)
                    {
                        SelectWaypointPath(obj);
                    }
                    var last = SelectedWaypointPaths.LastOrDefault();
                    MakeWaypointPathLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (WaypointPath obj in e.OldItems)
                    {
                        UnselectWaypointPath(obj);
                    }
                    var last = SelectedWaypointPaths.LastOrDefault();
                    MakeWaypointPathLastSelected(last);
                }
            };
            SelectedWorldPolygons.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (WorldPolygon obj in e.NewItems)
                    {
                        SelectWorldPolygon(obj);
                    }
                    var last = SelectedWorldPolygons.LastOrDefault();
                    MakeWorldPolygonLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (WorldPolygon obj in e.OldItems)
                    {
                        UnselectWorldPolygon(obj);
                    }
                    var last = SelectedWorldPolygons.LastOrDefault();
                    MakeWorldPolygonLastSelected(last);
                }
            };
            SelectedWorldPointSets.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (WorldPointSet obj in e.NewItems)
                    {
                        SelectWorldPointSet(obj);
                    }
                    var last = SelectedWorldPointSets.LastOrDefault();
                    MakeWorldPointSetLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (WorldPointSet obj in e.OldItems)
                    {
                        UnselectWorldPointSet(obj);
                    }
                    var last = SelectedWorldPointSets.LastOrDefault();
                    MakeWorldPointSetLastSelected(last);
                }
            };
            SelectedObjectivePoints.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (ObjectivePoint obj in e.NewItems)
                    {
                        SelectObjectivePoint(obj);
                    }
                    var last = SelectedObjectivePoints.LastOrDefault();
                    MakeObjectivePointLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (ObjectivePoint obj in e.OldItems)
                    {
                        UnselectObjectivePoint(obj);
                    }
                    var last = SelectedObjectivePoints.LastOrDefault();
                    MakeObjectivePointLastSelected(last);
                }
            };
            SelectedMapTextPoints.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (MapTextPoint obj in e.NewItems)
                    {
                        SelectMapTextPoint(obj);
                    }
                    var last = SelectedMapTextPoints.LastOrDefault();
                    MakeMapTextPointLastSelected(last);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (MapTextPoint obj in e.OldItems)
                    {
                        UnselectMapTextPoint(obj);
                    }
                    var last = SelectedMapTextPoints.LastOrDefault();
                    MakeMapTextPointLastSelected(last);
                }
            };
        }

        #region MenuCommands

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
                    ClearSelections();
                    var _lock = new object();
                    Map.EnableCollectionSynchronization(_lock);
                    new ProgressDialog(this, "Import map").RunActionSameThread((progress, progressLogs) =>
                    {
                        using var di = new DataImport(ofd.FileName, Map, progressLogs, progress, _lock);
                        di.ReadMapFileAndAddData();
                    });
                    Map.DisableCollectionSynchronization();
                }
            }
        }

        [RelayCommand]
        private void OnMapExport()
        {
            var sfd = new SaveFileDialog()
            {
                DefaultExt = ".twt",
                Filter = "Map file (.twt)|*.twt",
                Title = "Save your map file",
            };
            if (sfd.ShowDialog(this) == true)
            {
                new ProgressDialog(this, "Export map").RunAction((progress, progressLogs) =>
                {
                    using (var de = new DataExport(sfd.FileName, Map, progressLogs, progress))
                    {
                        de.CreateMapFileAndWriteData();
                    }
                });
            }
        }

        [RelayCommand]
        private void OnResetMap()
        {
            if (MessageBox.Show("The current map will be cleared. Continue ?", "Map reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ClearSelections();
                Map.Reset();
            }
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(this, "Map size", Map.Size, Map.ZSize, Map.WorldBuffer);
            if (msd.ShowDialog() == true)
            {
                Map.Size = msd.Size;
                Map.ZSize = msd.ZSize;
                Map.WorldBuffer = msd.WorldBuffer;
            }
        }

        [RelayCommand]
        private void OnWorldInfoEdit()
        {
            new WorldInfoDialog(this, "World info", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnFlagsEdit()
        {
            new FlagDialog(this, "Flags", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnGroupsEdit()
        {
            new CollectionEditorDialog(this, "Groups", Map.Groups, () => new Group(Map)).ShowDialog();
        }

        [RelayCommand]
        private void OnJournalEntriesEdit()
        {
            new JournalEntryDialog(this, "Journal entries", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnMapTextPointsEdit()
        {
            new CollectionEditorDialog(this, "Map text points", Map.MapTextPoints, () => new MapTextPoint(Map, NamedMapObject.GenerateName("MapTextPoint", Map.MapTextPoints), StringDictionnary.MapTextItems.Keys.FirstOrDefault())).ShowDialog();
            if (SelectedMapTextPoint != null)
            {
                if (!Map.MapTextPoints.Contains(SelectedMapTextPoint))
                {
                    SelectedMapTextPoint = null;
                    if (MapTextPointRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnObjectiveTasksEdit()
        {
            new CollectionEditorDialog(this, "Objective tasks", Map.ObjectiveTasks, () => new ObjectiveTask(Map, NamedMapObject.GenerateName("ObjectiveTask", Map.ObjectiveTasks), StringDictionnary.ObjectiveTasks.Keys.FirstOrDefault())).ShowDialog();
        }

        [RelayCommand]
        private void OnObjectivePointsEdit()
        {
            new CollectionEditorDialog(this, "Objective points", Map.ObjectivePoints, () => new ObjectivePoint(Map)).ShowDialog();
            if (SelectedObjectivePoint != null)
            {
                if (!Map.ObjectivePoints.Contains(SelectedObjectivePoint))
                {
                    SelectedObjectivePoint = null;
                    if (ObjectivePointRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new CollectionEditorDialog(this, "Players", Map.Players, () => new Player(Map)).ShowDialog();
            if (SelectedPlayer != null)
            {
                if (!Map.Players.Contains(SelectedPlayer))
                {
                    SelectedPlayer = null;
                    if (PlayerRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            if (Map.Players.Count > 1)
                new PlayerAllianceDialog(this, "Player alliances", Map).ShowDialog();
            else
                MessageBox.Show("You need at least 2 players to create alliances.");
        }

        [RelayCommand]
        private void OnSpeechEventsEdit()
        {
            new CollectionEditorDialog(this, "Speech events", Map.SpeechEvents, () => new SpeechEvent(Map, NamedMapObject.GenerateName("SpeechEvent", Map.SpeechEvents))).ShowDialog();
        }

        [RelayCommand]
        private void OnTeamsEdit()
        {
            new TeamsDialog(this, "Teams", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnTimersEdit()
        {
            new TimerDialog(this, "Timers", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWaypointPathsEdit()
        {
            new CollectionEditorDialog(this, "Waypoint paths", Map.WaypointPaths, () =>
            {
                var wp = new WaypointPath(Map, NamedMapObject.GenerateName("WaypointPath", Map.WaypointPaths));
                wp.Points.Add(new(wp, 0, 0, 0));
                return wp;
            }).ShowDialog();
            if (SelectedWaypointPath != null)
            {
                if (!Map.WaypointPaths.Contains(SelectedWaypointPath))
                {
                    SelectedWaypointPathPoint = null;
                    SelectedWaypointPath = null;
                    if (WaypointPathRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if (SelectedWaypointPathPoint != null && !SelectedWaypointPath.Points.Contains(SelectedWaypointPathPoint))
                {
                    SelectedWaypointPathPoint = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldCrewsAndArmsEdit()
        {
            new WorldCrewAndArmsDialog(this, "World crews and arms", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new CollectionEditorDialog(this, "World objects", Map.WorldObjects, () => new WorldObject(Map)).ShowDialog();
            if (SelectedWorldObject != null)
            {
                if (!Map.WorldObjects.Contains(SelectedWorldObject))
                {
                    SelectedWorldObject = null;
                    if (WorldObjectRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldPointSetsEdit()
        {
            new CollectionEditorDialog(this, "World point sets", Map.WorldPointSets, () =>
            {
                var wps = new WorldPointSet(Map, NamedMapObject.GenerateName("WorldPointSet", Map.WorldPointSets));
                wps.Points.Add(new(wps, 0, 0, 0, 0));
                return wps;
            }).ShowDialog();
            if (SelectedWorldPointSet != null)
            {
                if (!Map.WorldPointSets.Contains(SelectedWorldPointSet))
                {
                    SelectedWorldPoint = null;
                    SelectedWorldPointSet = null;
                    if (WorldPointSetRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if(SelectedWorldPoint!=null && !SelectedWorldPointSet.Points.Contains(SelectedWorldPoint))
                {
                    SelectedWorldPoint = null;
                    if (WorldPointSetRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldPolygonsEdit()
        {
            new CollectionEditorDialog(this, "World polygons", Map.WorldPolygons, () =>
            {
                var wp = new WorldPolygon(Map, NamedMapObject.GenerateName("WorldPolygon", Map.WorldPolygons));
                wp.Points.Add(new(wp, 0, 0));
                return wp;
            }).ShowDialog();
            if (SelectedWorldPolygon != null)
            {
                if (!Map.WorldPolygons.Contains(SelectedWorldPolygon))
                {
                    SelectedWorldPolygonPoint = null;
                    SelectedWorldPolygon = null;
                    if (WorldPolygonRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if (SelectedWorldPolygonPoint != null && !SelectedWorldPolygon.Points.Contains(SelectedWorldPolygonPoint))
                {
                    SelectedWorldPolygonPoint = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            new CollectionEditorDialog(this, "World rules", Map.WorldRules, () => new WorldRule(Map, NamedMapObject.GenerateName("WorldRule", Map.WorldRules))).ShowDialog();
        }

        [RelayCommand]
        private void OnAppSettingsEdit()
        {
            var tpGamePath = settings.TpGamePath;
            var asd = new AppSettingsDialog(this, "Settings", settings);
            asd.ShowDialog();
            settings.Save();
            if (settings.TpGamePath != tpGamePath)
            {
                new ProgressDialog(this, "Reload TPGame folder").RunActionSameThread((progress, progressLogs) =>
                {
                    progress.Report("Reloading ...");
                    settings.ReloadAll(progress, progressLogs);
                    progress.Report("Reloading complete");
                }, true);
            }
        }

        [RelayCommand]
        private void OnReloadAll()
        {
            ReloadAllSettings();
        }

        [RelayCommand]
        private void OnReloadDialogueFilesList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadDialogueFilesList();
                    progress.Report("Reloading complete");
                }
                catch(Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadEffectList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadEffectList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadFlagTexturesList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadFlagTexturesList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadGuiTexturesList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadGuiTexturesList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadHudTexturesList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadHudTexturesList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadMeshesList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadMeshesList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadMusicsList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadMusicsList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadSinglePlayerMissionsList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadSinglePlayerMissionsList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadStrings()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadStringsDictionnaries(progress, logs);
                    progress.Report("Reloading complete");
                    MapTextPointPreviewTextComboBox.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadWorldObjectTypeList()
        {
            new ProgressDialog(this, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadWorldObjectTypeList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnAboutAppShow()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"TPMapEditor version {v.Major}.{v.Minor}.{v.Build}\nAuthor : Randhomme", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region KeyboardShortcutCommands

        [RelayCommand]
        private void OnHKey()
        {
            GetKeyboardShortcutApplier()?.OnHKey();
        }

        [RelayCommand]
        private void OnShiftHKey()
        {
            GetKeyboardShortcutApplier()?.OnShiftHKey();
        }

        [RelayCommand]
        private void OnCtrlHKey()
        {
            GetKeyboardShortcutApplier()?.OnCtrlHKey();
        }

        [RelayCommand]
        private void OnAKey()
        {
            GetKeyboardShortcutApplier()?.OnAKey();
        }

        [RelayCommand]
        private void OnShiftAKey()
        {
            GetKeyboardShortcutApplier()?.OnShiftAKey();
        }

        [RelayCommand]
        private void OnCtrlAKey()
        {
            GetKeyboardShortcutApplier()?.OnCtrlAKey();
        }

        #endregion

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
            settings = settings.Load();
            if (string.IsNullOrEmpty(settings.TpGamePath))
            {
                MessageBox.Show("You should set the TPGame path in the application settings before using the map editor.", "TPGame Path Not Set", MessageBoxButton.OK, MessageBoxImage.Warning);
                OnAppSettingsEdit();
            }
            else
            {
                ReloadAllSettings(false);
            }
        }

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            if (!ctrlPressed)
                return;

            if (e.Delta < 0 && MapScrollViewer.ScrollableHeight == 0 && MapScrollViewer.ScrollableWidth == 0)
                return;

            const double zoomFactor = 1.1;
            double scale = (e.Delta > 0) ? zoomFactor : 1 / zoomFactor;

            Point mousePos = e.GetPosition(MapScrollViewer);

            double absoluteX = MapScrollViewer.HorizontalOffset + mousePos.X;
            double absoluteY = MapScrollViewer.VerticalOffset + mousePos.Y;
            double realScale = MapViewBox.ActualWidth / MapGrid.ActualWidth;
            double newScale = realScale * scale;

            ZoomTransform.ScaleX = newScale;
            ZoomTransform.ScaleY = newScale;

            double newAbsoluteX = absoluteX * scale;
            double newAbsoluteY = absoluteY * scale;

            MapScrollViewer.ScrollToHorizontalOffset(newAbsoluteX - mousePos.X);
            MapScrollViewer.ScrollToVerticalOffset(newAbsoluteY - mousePos.Y);

            e.Handled = true;
        }

        private void ClearSelections()
        {
            SelectedWorldObjects.Clear();
            SelectedPlayers.Clear();
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPaths.Clear();
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygons.Clear();
            SelectedWorldPoints.Clear();
            SelectedWorldPointSets.Clear();
            SelectedObjectivePoints.Clear();
            SelectedMapTextPoints.Clear();
            SelectedWorldObject = null;
            SelectedPlayer = null;
            SelectedWaypointPathPoint = null;
            SelectedWaypointPath = null;
            SelectedWorldPolygonPoint = null;
            SelectedWorldPolygon = null;
            SelectedWorldPoint = null;
            SelectedWorldPointSet = null;
            SelectedObjectivePoint = null;
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void IncreaseZIndexButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetZIndex(SelectedElement, Panel.GetZIndex(SelectedElement) + 1);
            //if (SelectedElement != null)
            //    SelectedElement.Opacity += 0.1;
        }

        private void DecreaseZIndexButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetZIndex(SelectedElement, Panel.GetZIndex(SelectedElement) - 1);
            //if (SelectedElement != null)
            //    SelectedElement.Opacity -= 0.1;
        }

        //Disable default Alt behaviour to allow rotation
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                (RotateCheckBox.IsChecked == true ||
                (WorldObjectPreviewControl.Visibility == Visibility.Visible) ||
                (PlayerPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointSetPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointPreviewControl.Visibility == Visibility.Visible)))
                e.Handled = true;
        }

        //Scroll horizontally by pressing Shift and using MouseWheel
        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                MapScrollViewer.ScrollToHorizontalOffset(MapScrollViewer.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private int GetAcceleratedRotation()
        {
            DateTime now = DateTime.Now;
            double ms = (now - lastWheelTime).TotalMilliseconds;
            lastWheelTime = now;

            if (ms < 25) return 10;
            if (ms < 50) return 5;
            if (ms < 100) return 2;

            return 1;
        }

        //Returns a rotation between -180 and 180
        private double GetRotation(double rotation)
        {
            if (rotation > 180) rotation -= 360;
            else if (rotation < -180) rotation += 360;
            return rotation;
        }

        private bool IsSelectableWorldObjectType(object o)
        {
            if(o is WorldObjectType wot)
            {
                return wot.CustomInfoDefinition == CustomInfoDefinition.AsteroidCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.BlackHoleCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.BulletCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.DragonCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.EtheriumCurrentCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.IslandCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.MineCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.NebulaCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.NovaMortarCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.ShipCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.ShipDebrisCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.SpaceAnimalCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.StarMortarCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.TorpedoCustomInfoFactory;
            }
            return false;
            
        }

        private void MapScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            MapScrollViewer.Focus();
        }

        private void ReloadAllSettings(bool notifyOnFinish = true)
        {
            new ProgressDialog(this, "Reload TPGame folder").RunActionSameThread((progress, progressLogs) =>
            {
                progress.Report("Reloading ...");
                settings.ReloadAll(progress, progressLogs);
                progress.Report("Reloading complete");
                MapTextPointPreviewTextComboBox.SelectedIndex = 0;
            }, true, notifyOnFinish);
        }

        private KeyboardShortcutApplier? GetKeyboardShortcutApplier()
        {
            if (WorldObjectRadioButton.IsChecked == true)
            {
                return worldObjectKeyboardShortcutApplier;
            }
            else if (PlayerRadioButton.IsChecked == true)
            {
                return playerKeyboardShortcutApplier;
            }
            else if (WaypointPathRadioButton.IsChecked == true)
            {
                return waypointPathKeyboardShortcutApplier;
            }
            else if (WorldPolygonRadioButton.IsChecked == true)
            {
                return worldPolygonKeyboardShortcutApplier;
            }
            else if (WorldPointSetRadioButton.IsChecked == true)
            {
                return worldPointSetKeyboardShortcutApplier;
            }
            else if (ObjectivePointRadioButton.IsChecked == true)
            {
                return objectivePointKeyboardShortcutApplier;
            }
            else if (MapTextPointRadioButton.IsChecked == true)
            {
                return mapTextPointKeyboardShortcutApplier;
            }
            return null;
        }

        //private void OnSelectionCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (ISelectableMapObject obj in e.NewItems)
        //        {
        //            SelectObject(obj);
        //        }
        //        var last = SelectedPlayers.LastOrDefault();
        //        MakeObjectLastSelected(last);
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (ISelectableMapObject obj in e.OldItems)
        //        {
        //            UnselecObject(obj);
        //        }
        //        var last = SelectedPlayers.LastOrDefault();
        //        MakeObjectLastSelected(last);
        //    }
        //}

        #endregion

        #region WorldObject

        private void WorldObjectRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldObjectElements();
        }

        private void WorldObjectRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldObjectElements();
        }

        private void ShowWorldObjectElements()
        {
            WorldObjectGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldObjectItemsControl, 1);
            WorldObjectItemsControl.Opacity = 1;
            WorldObjectItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldObject();
            RotateCheckBox.Checked += RotateWorldObjectCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldObjectCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldObject();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click += DeleteWorldObjectButton_Click;
            SelectedElement = (UIElement)WorldObjectItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldObject);
        }

        private void HideWorldObjectElements()
        {
            WotDataGrid.SelectedItem = null;
            WorldObjectGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldObjectItemsControl, 0);
            WorldObjectItemsControl.Opacity = 0.5;
            WorldObjectItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldObject();
            RotateCheckBox.Checked -= RotateWorldObjectCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotateWorldObjectCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotateWorldObject();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldObjectButton_Click;
        }

        private void MapGridOutsideWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldObjectSelection();
        }

        private void OnWorldObjectClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldObject clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldObject(clickedObject);
                }
                else
                {
                    ClearWorldObjectSelection();
                    SelectAndMakeWorldObjectLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void CtrlSelectWorldObject(WorldObject worldObject)
        {
            if (worldObject.IsLastSelected)
            {
                RemoveWorldObjectFromSelection(worldObject);
            }
            else
            {
                SelectAndMakeWorldObjectLastSelected(worldObject);
            }
        }

        private void SelectAndMakeWorldObjectLastSelected(WorldObject worldObject)
        {
            AddWorldObjectToSelection(worldObject);
            MakeWorldObjectLastSelected(worldObject);
        }

        private void SelectWorldObject(WorldObject worldObject)
        {
            worldObject.IsSelected = true;
        }

        private void AddWorldObjectToSelection(WorldObject worldObject)
        {
            if (!worldObject.IsSelected)
            {
                worldObject.IsSelected = true;
                SelectedWorldObjects.Add(worldObject);
            }
        }

        private void MakeWorldObjectLastSelected(WorldObject worldObject)
        {
            if (SelectedWorldObject != null)
            {
                SelectedWorldObject.IsLastSelected = false;
            }
            SelectedWorldObject = worldObject;
            if(SelectedWorldObject != null)
            {
                SelectedWorldObject.IsLastSelected = true;
                SelectedElement = (UIElement)WorldObjectItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldObject);
            }
        }

        private void UnselectWorldObject(WorldObject worldObject)
        {
            worldObject.IsSelected = false;
        }

        private void RemoveWorldObjectFromSelection(WorldObject worldObject)
        {
            UnselectWorldObject(worldObject);
            SelectedWorldObjects.Remove(worldObject);
        }

        private void ClearWorldObjectSelection()
        {
            foreach (var v in SelectedWorldObjects)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
            SelectedElement = null;
        }

        private void MoveWorldObjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldObject();
        }

        private void MoveWorldObjectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldObject();
        }

        private void EnableMoveWorldObject()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldObject()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldObject_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldObject_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldObject_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldObjects.Count; i++)
            {
                var selectedObject = SelectedWorldObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldObjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedWorldObjects.ToArray();
            foreach(var item in selectedItems)
            {
                Map.WorldObjects.Remove(item);
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
            SelectedElement = null;
        }

        private void MapGridOutsideWorldObjectPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldObjectPreviewControl, mousePos.X - WorldObjectPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldObjectPreviewControl, mousePos.Y - WorldObjectPreviewControl.ActualHeight / 2);
        }

        private void WorldObjectPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var wot = new WorldObject(Map, SelectedWorldObjectType!, Canvas.GetLeft(WorldObjectPreviewControl) + WorldObjectPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldObjectPreviewControl) - WorldObjectPreviewControl.ActualHeight / 2, WotSliderRotate.Value);
            Map.WorldObjects.Add(wot);
            SelectAndMakeWorldObjectLastSelected(wot);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldObjectPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WotDataGrid.SelectedItem = null;
        }

        private void WorldObjectPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = WotSliderRotate.Value + (e.Delta > 0 ? step : -step);
                WotSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
        }

        private void RotateWorldObjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotateWorldObject();
        }

        private void RotateWorldObjectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotateWorldObject();
        }

        private void EnableRotateWorldObject()
        {
            MapGridOutside.MouseWheel += MapGridOutsideWorldObject_MouseWheel;
        }

        private void DisableRotateWorldObject()
        {
            MapGridOutside.MouseWheel -= MapGridOutsideWorldObject_MouseWheel;
        }

        private void MapGridOutsideWorldObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedWorldObjects.Count; i++)
                {
                    var worldObject = SelectedWorldObjects[i];
                    var newRotation = worldObject.ZRotation + (e.Delta > 0 ? step : -step);
                    worldObject.ZRotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        private void WorldObjectVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldObjects)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void WorldObjectVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldObjects)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region Player

        private void PlayerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPlayerElements();
        }

        private void PlayerRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePlayerElements();
        }

        private void ShowPlayerElements()
        {
            PlayerGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(PlayerItemsControl, 1);
            PlayerItemsControl.Opacity = 1;
            PlayerItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked += MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMovePlayer();
            RotateCheckBox.Checked += RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked += RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotatePlayer();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click += DeletePlayerButton_Click;
            SelectedElement = (UIElement)PlayerItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedPlayer);
        }

        private void HidePlayerElements()
        {
            WotDataGrid.SelectedItem = null;
            PlayerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(PlayerItemsControl, 0);
            PlayerItemsControl.Opacity = 0.5;
            PlayerItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked -= MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMovePlayer();
            RotateCheckBox.Checked -= RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotatePlayer();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click -= DeletePlayerButton_Click;
        }

        private void MapGridOutsidePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearPlayerSelection();
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is Player clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectPlayer(clickedObject);
                }
                else
                {
                    ClearPlayerSelection();
                    SelectAndMakePlayerLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void CtrlSelectPlayer(Player player)
        {
            if (player.IsLastSelected)
            {
                RemovePlayerFromSelection(player);
            }
            else
            {
                SelectAndMakePlayerLastSelected(player);
            }
        }

        private void SelectAndMakePlayerLastSelected(Player player)
        {
            AddPlayerToSelection(player);
            MakePlayerLastSelected(player);
        }

        private void SelectPlayer(Player player)
        {
            player.IsSelected = true;
        }

        private void AddPlayerToSelection(Player player)
        {
            if (!player.IsSelected)
            {
                player.IsSelected = true;
                SelectedPlayers.Add(player);
            }
        }

        private void MakePlayerLastSelected(Player player)
        {
            if (SelectedPlayer != null)
            {
                SelectedPlayer.IsLastSelected = false;
            }
            SelectedPlayer = player;
            if (SelectedPlayer != null)
            {
                SelectedPlayer.IsLastSelected = true;
                SelectedElement = (UIElement)PlayerItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedPlayer);
            }
        }

        private void UnselectPlayer(Player player)
        {
            player.IsSelected = false;
        }

        private void RemovePlayerFromSelection(Player player)
        {
            UnselectPlayer(player);
            SelectedPlayers.Remove(player);
        }

        private void ClearPlayerSelection()
        {
            foreach (var v in SelectedPlayers)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
            SelectedElement = null;
        }

        private void MovePlayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMovePlayer();
        }

        private void MovePlayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMovePlayer();
        }

        private void EnableMovePlayer()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMovePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMovePlayer_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMovePlayer()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMovePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMovePlayer_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMovePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMovePlayer_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMovePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMovePlayer_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMovePlayer_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedPlayers.Count; i++)
            {
                var selectedObject = SelectedPlayers[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedPlayers.ToArray();
            foreach (var item in selectedItems)
            {
                Map.Players.Remove(item);
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
            SelectedElement = null;
        }

        private void MapGridOutsidePlayerPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(PlayerPreviewControl, mousePos.X - PlayerPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PlayerPreviewControl, mousePos.Y - PlayerPreviewControl.ActualHeight / 2);
        }

        private void PlayerPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var player = new Player(Map, NamedMapObject.GenerateName("Player", Map.Players), Canvas.GetLeft(PlayerPreviewControl) + PlayerPreviewControl.ActualWidth / 2, -Canvas.GetTop(PlayerPreviewControl) - PlayerPreviewControl.ActualHeight / 2, 0, PlayerSliderRotate.Value, Colors.Red);
            Map.Players.Add(player);
            SelectAndMakePlayerLastSelected(player);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void PlayerPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPlayerCheckBox.IsChecked = false;
        }

        private void PlayerPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = WotSliderRotate.Value + (e.Delta > 0 ? step : -step);
                WotSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
        }

        private void RotatePlayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotatePlayer();
        }

        private void RotatePlayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotatePlayer();
        }

        private void EnableRotatePlayer()
        {
            MapGridOutside.MouseWheel += MapGridOutsidePlayer_MouseWheel;
        }

        private void DisableRotatePlayer()
        {
            MapGridOutside.MouseWheel -= MapGridOutsidePlayer_MouseWheel;
        }

        private void MapGridOutsidePlayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedPlayers.Count; i++)
                {
                    var player = SelectedPlayers[i];
                    var newRotation = player.Rotation + (e.Delta > 0 ? step : -step);
                    player.Rotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        private void PlayerVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.Players)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void PlayerVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.Players)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region WaypointPath

        private void WaypointPathRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWaypointPathElements();
        }

        private void WaypointPathRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWaypointPathElements();
        }

        private void ShowWaypointPathElements()
        {
            WaypointPathGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WaypointPathItemsControl, 1);
            WaypointPathItemsControl.Opacity = 1;
            WaypointPathItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWaypointPathPointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click += DeleteWaypointPathPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWaypointPathPoint();
            SelectedElement = (UIElement)WaypointPathItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWaypointPath);
        }

        private void HideWaypointPathElements()
        {
            WaypointPathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WaypointPathItemsControl, 0);
            WaypointPathItemsControl.Opacity = 0.5;
            WaypointPathItemsControl.IsEnabled = false;
            NewWaypointPathRadioButton.IsChecked = AddWaypointPathPointRadioButton.IsChecked = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            MoveCheckBox.Checked -= MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWaypointPathPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWaypointPathPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWaypointPathPointButton_Click;
        }

        private void OnWaypointPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPath clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWaypointPath(clickedObject);
                }
                else
                {
                    ClearWaypointPathSelection();
                    AddWaypointPathToSelection(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPathPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWaypointPathPoint(clickedObject);
                }
                else
                {
                    ClearWaypointPathSelection();
                    AddWaypointPathPointToSelection(clickedObject, false);
                    MakeWaypointPathPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWaypointPathSelection();
        }
        
        private void CtrlSelectWaypointPath(WaypointPath waypointPath)
        {
            if (waypointPath.IsSelected)
            {
                if (waypointPath.IsLastSelected)
                {
                    RemoveWaypointPathFromSelection(waypointPath);
                }
                else
                {
                    MakeWaypointPathLastSelected(waypointPath);
                }
            }
            else
            {
                AddWaypointPathToSelection(waypointPath); //Make last selected from ObservableCollection on CollectionChanged
            }
        }

        private void RemoveWaypointPathFromSelection(WaypointPath waypointPath)
        {
            SelectedWaypointPaths.Remove(waypointPath);
        }

        private void UnselectWaypointPath(WaypointPath waypointPath)
        {
            waypointPath.IsSelected = false;
            foreach (var p in waypointPath.Points)
            {
                RemoveWaypointPathPointFromSelection(p);
            }
        }

        private void MakeWaypointPathLastSelected(WaypointPath waypointPath)
        {
            if (SelectedWaypointPath != null)
            {
                SelectedWaypointPath.IsLastSelected = false;
            }
            SelectedWaypointPath = waypointPath;
            if (SelectedWaypointPath != null)
            {
                SelectedWaypointPath.IsLastSelected = true;
                SelectedElement = (UIElement)WaypointPathItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWaypointPath);
            }
        }

        private void AddWaypointPathToSelection(WaypointPath waypointPath)
        {
            if (!waypointPath.IsSelected)
                SelectedWaypointPaths.Add(waypointPath);
        }

        private void AddWaypointPathNoPointsToSelection(WaypointPath waypointPath)
        {
            if (!waypointPath.IsSelected)
            {
                var selectedPoints = waypointPath.Points.Where((p) => p.IsSelected == true).ToArray(); //Save current selection
                SelectedWaypointPaths.Add(waypointPath); //Triggers selection on all points
                foreach(var p in waypointPath.Points)
                {
                    if (p.IsSelected && !selectedPoints.Contains(p))
                    {
                        RemoveWaypointPathPointFromSelection(p);
                    }
                    else if(!p.IsSelected && selectedPoints.Contains(p))
                    {
                        AddWaypointPathPointToSelection(p);
                    }
                }
            }
        }

        private void SelectWaypointPath(WaypointPath waypointPath)
        {
            waypointPath.IsSelected = true;
            foreach(var p in waypointPath.Points)
            {
                AddWaypointPathPointToSelection(p);
            }
        }

        private void AddWaypointPathPointToSelection(WaypointPathPoint waypointPathPoint, bool selectOtherPathPoints = true)
        {
            if (!waypointPathPoint.IsSelected)
            {
                SelectWaypointPathPoint(waypointPathPoint);
                SelectedWaypointPathPoints.Add(waypointPathPoint);
                if (selectOtherPathPoints)
                    AddWaypointPathToSelection(waypointPathPoint.Parent);
                else
                    AddWaypointPathNoPointsToSelection(waypointPathPoint.Parent);
            }
        }

        private void SelectWaypointPathPoint(WaypointPathPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = true;
        }

        private void RemoveWaypointPathPointFromSelection(WaypointPathPoint waypointPathPoint)
        {
            UnselectWaypointPathPoint(waypointPathPoint);
            SelectedWaypointPathPoints.Remove(waypointPathPoint);
            if(!HasWaypointPathOneSelectedPoint(waypointPathPoint.Parent))
                RemoveWaypointPathFromSelection(waypointPathPoint.Parent);
        }

        private bool HasWaypointPathOneSelectedPoint(WaypointPath waypointPath)
        {
            foreach(var p in waypointPath.Points)
            {
                if (p.IsSelected) return true;
            }
            return false;
        }

        private void UnselectWaypointPathPoint(WaypointPathPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = false;
            if (waypointPathPoint.IsLastSelected)
            {
                waypointPathPoint.IsLastSelected = false;
                SelectedWaypointPathPoint = null;
            }
        }

        private void CtrlSelectWaypointPathPoint(WaypointPathPoint waypointPathPoint)
        {
            if (waypointPathPoint.IsSelected)
            {
                if (waypointPathPoint.IsLastSelected)
                {
                    RemoveWaypointPathPointFromSelection(waypointPathPoint);
                }
                else
                {
                    MakeWaypointPathPointLastSelected(waypointPathPoint);
                }
            }
            else
            {
                AddWaypointPathPointToSelection(waypointPathPoint, false);
                MakeWaypointPathPointLastSelected(waypointPathPoint);
            }
        }

        private void MakeWaypointPathPointLastSelected(WaypointPathPoint waypointPathPoint)
        {
            if (SelectedWaypointPathPoint != null)
            {
                SelectedWaypointPathPoint.IsLastSelected = false;
            }
            waypointPathPoint.IsLastSelected = true;
            SelectedWaypointPathPoint = waypointPathPoint;
            MakeWaypointPathLastSelected(waypointPathPoint.Parent);
        }

        private void ClearWaypointPathSelection()
        {
            foreach (var v in SelectedWaypointPaths)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWaypointPaths.Clear();
            SelectedWaypointPath = null;
            SelectedElement = null;
            ClearWaypointPathPointSelection();
        }

        private void ClearWaypointPathPointSelection()
        {
            foreach (var v in SelectedWaypointPathPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPathPoint = null;
        }

        private void MoveWaypointPathPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWaypointPathPoint();
        }

        private void MoveWaypointPathPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWaypointPathPoint();
        }

        private void EnableMoveWaypointPathPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWaypointPathPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWaypointPathPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWaypointPathPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWaypointPathPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWaypointPathPoints.Count; i++)
            {
                var selectedObject = SelectedWaypointPathPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedWaypointPathPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if(p.Parent.Points.Count == 0)
                {
                    Map.WaypointPaths.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWaypointPath = null;
                        SelectedElement = null;
                    }
                }
            }
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPathPoint = null;
        }

        private void MapGridOutsideWaypointPathPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WaypointPathPreviewControl, mousePos.X - WaypointPathPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WaypointPathPreviewControl, mousePos.Y - WaypointPathPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WaypointPathPointPreviewControl, mousePos.X - WaypointPathPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WaypointPathPointPreviewControl, mousePos.Y - WaypointPathPointPreviewControl.ActualHeight / 2);
        }

        private void WaypointPathPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var waypointPath = new WaypointPath(Map, NamedMapObject.GenerateName("WaypointPath", Map.WaypointPaths));
            var point = new WaypointPathPoint(waypointPath, Canvas.GetLeft(WaypointPathPreviewControl) + WaypointPathPreviewControl.ActualWidth / 2, -Canvas.GetTop(WaypointPathPreviewControl) - WaypointPathPreviewControl.ActualHeight / 2, 0);
            waypointPath.Points.Add(point);
            Map.WaypointPaths.Add(waypointPath);
            ClearWaypointPathSelection();
            AddWaypointPathToSelection(waypointPath);
            MakeWaypointPathPointLastSelected(point);
            AddWaypointPathPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NewWaypointPathRadioButton.IsChecked = false;
        }

        private void WaypointPathPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWaypointPath != null)
            {
                var point = new WaypointPathPoint(SelectedWaypointPath, Canvas.GetLeft(WaypointPathPointPreviewControl) + WaypointPathPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WaypointPathPointPreviewControl) - WaypointPathPointPreviewControl.ActualHeight / 2, 0);
                AddWaypointPathPointToSelection(point, false);
                MakeWaypointPathPointLastSelected(point);
                SelectedWaypointPath.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWaypointPathPointRadioButton.IsChecked = false;
        }

        private void WaypointPathVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WaypointPaths)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void WaypointPathVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WaypointPaths)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region WorldPolygon

        private void WorldPolygonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPolygonElements();
        }

        private void WorldPolygonRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPolygonElements();
        }

        private void ShowWorldPolygonElements()
        {
            WorldPolygonGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPolygonItemsControl, 1);
            WorldPolygonItemsControl.Opacity = 1;
            WorldPolygonItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPolygonPointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click += DeleteWorldPolygonPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPolygonPoint();
            SelectedElement = (UIElement)WorldPolygonItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPolygon);
        }

        private void HideWorldPolygonElements()
        {
            WorldPolygonGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPolygonItemsControl, 0);
            WorldPolygonItemsControl.Opacity = 0.5;
            WorldPolygonItemsControl.IsEnabled = false;
            NewWorldPolygonRadioButton.IsChecked = AddWorldPolygonPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPolygonPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPolygonPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPolygonPointButton_Click;
        }

        private void OnWorldPolygonClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygon clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPolygon(clickedObject);
                }
                else
                {
                    ClearWorldPolygonSelection();
                    AddWorldPolygonToSelection(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWorldPolygonPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygonPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPolygonPoint(clickedObject);
                }
                else
                {
                    ClearWorldPolygonSelection();
                    AddWorldPolygonPointToSelection(clickedObject, false);
                    MakeWorldPolygonPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldPolygonSelection();
        }

        private void CtrlSelectWorldPolygon(WorldPolygon worldPolygon)
        {
            if (worldPolygon.IsSelected)
            {
                if (worldPolygon.IsLastSelected)
                {
                    RemoveWorldPolygonFromSelection(worldPolygon);
                }
                else
                {
                    MakeWorldPolygonLastSelected(worldPolygon);
                }
            }
            else
            {
                AddWorldPolygonToSelection(worldPolygon); //Make last selected from ObservableCollection on CollectionChanged
            }
        }

        private void RemoveWorldPolygonFromSelection(WorldPolygon worldPolygon)
        {
            SelectedWorldPolygons.Remove(worldPolygon);
        }

        private void UnselectWorldPolygon(WorldPolygon worldPolygon)
        {
            worldPolygon.IsSelected = false;
            foreach (var p in worldPolygon.Points)
            {
                RemoveWorldPolygonPointFromSelection(p);
            }
        }

        private void MakeWorldPolygonLastSelected(WorldPolygon worldPolygon)
        {
            if (SelectedWorldPolygon != null)
            {
                SelectedWorldPolygon.IsLastSelected = false;
            }
            SelectedWorldPolygon = worldPolygon;
            if (SelectedWorldPolygon != null)
            {
                SelectedWorldPolygon.IsLastSelected = true;
                SelectedElement = (UIElement)WorldPolygonItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPolygon);
            }
        }

        private void AddWorldPolygonToSelection(WorldPolygon worldPolygon)
        {
            if (!worldPolygon.IsSelected)
                SelectedWorldPolygons.Add(worldPolygon);
        }

        private void AddWorldPolygonNoPointsToSelection(WorldPolygon worldPolygon)
        {
            if (!worldPolygon.IsSelected)
            {
                var selectedPoints = worldPolygon.Points.Where((p) => p.IsSelected == true).ToArray(); //Save current selection
                SelectedWorldPolygons.Add(worldPolygon); //Triggers selection on all points
                foreach (var p in worldPolygon.Points)
                {
                    if (p.IsSelected && !selectedPoints.Contains(p))
                    {
                        RemoveWorldPolygonPointFromSelection(p);
                    }
                    else if (!p.IsSelected && selectedPoints.Contains(p))
                    {
                        AddWorldPolygonPointToSelection(p);
                    }
                }
            }
        }

        private void SelectWorldPolygon(WorldPolygon worldPolygon)
        {
            worldPolygon.IsSelected = true;
            foreach (var p in worldPolygon.Points)
            {
                AddWorldPolygonPointToSelection(p);
            }
        }

        private void AddWorldPolygonPointToSelection(WorldPolygonPoint worldPolygonPoint, bool selectOtherPathPoints = true)
        {
            if (!worldPolygonPoint.IsSelected)
            {
                SelectWorldPolygonPoint(worldPolygonPoint);
                SelectedWorldPolygonPoints.Add(worldPolygonPoint);
                if (selectOtherPathPoints)
                    AddWorldPolygonToSelection(worldPolygonPoint.Parent);
                else
                    AddWorldPolygonNoPointsToSelection(worldPolygonPoint.Parent);
            }
        }

        private void SelectWorldPolygonPoint(WorldPolygonPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = true;
        }

        private void RemoveWorldPolygonPointFromSelection(WorldPolygonPoint worldPolygonPoint)
        {
            UnselectWorldPolygonPoint(worldPolygonPoint);
            SelectedWorldPolygonPoints.Remove(worldPolygonPoint);
            if (!HasWorldPolygonOneSelectedPoint(worldPolygonPoint.Parent))
                RemoveWorldPolygonFromSelection(worldPolygonPoint.Parent);
        }

        private bool HasWorldPolygonOneSelectedPoint(WorldPolygon worldPolygon)
        {
            foreach (var p in worldPolygon.Points)
            {
                if (p.IsSelected) return true;
            }
            return false;
        }

        private void UnselectWorldPolygonPoint(WorldPolygonPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = false;
            if (worldPolygonPoint.IsLastSelected)
            {
                worldPolygonPoint.IsLastSelected = false;
                SelectedWorldPolygonPoint = null;
            }
        }

        private void CtrlSelectWorldPolygonPoint(WorldPolygonPoint worldPolygonPoint)
        {
            if (worldPolygonPoint.IsSelected)
            {
                if (worldPolygonPoint.IsLastSelected)
                {
                    RemoveWorldPolygonPointFromSelection(worldPolygonPoint);
                }
                else
                {
                    MakeWorldPolygonPointLastSelected(worldPolygonPoint);
                }
            }
            else
            {
                AddWorldPolygonPointToSelection(worldPolygonPoint, false);
                MakeWorldPolygonPointLastSelected(worldPolygonPoint);
            }
        }

        private void MakeWorldPolygonPointLastSelected(WorldPolygonPoint worldPolygonPoint)
        {
            if (SelectedWorldPolygonPoint != null)
            {
                SelectedWorldPolygonPoint.IsLastSelected = false;
            }
            worldPolygonPoint.IsLastSelected = true;
            SelectedWorldPolygonPoint = worldPolygonPoint;
            MakeWorldPolygonLastSelected(worldPolygonPoint.Parent);
        }

        private void ClearWorldPolygonSelection()
        {
            foreach (var v in SelectedWorldPolygons)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPolygons.Clear();
            SelectedWorldPolygon = null;
            SelectedElement = null;
            ClearWorldPolygonPointSelection();
        }

        private void ClearWorldPolygonPointSelection()
        {
            foreach (var v in SelectedWorldPolygonPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygonPoint = null;
        }

        private void MoveWorldPolygonPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldPolygonPoint();
        }

        private void MoveWorldPolygonPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldPolygonPoint();
        }

        private void EnableMoveWorldPolygonPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPolygonPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPolygonPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldPolygonPoints.Count; i++)
            {
                var selectedObject = SelectedWorldPolygonPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPolygonPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedWorldPolygonPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPolygons.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWorldPolygon = null;
                        SelectedElement = null;
                    }
                }
            }
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygonPoint = null;
        }

        private void MapGridOutsideWorldPolygonPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldPolygonPreviewControl, mousePos.X - WorldPolygonPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPolygonPreviewControl, mousePos.Y - WorldPolygonPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WorldPolygonPointPreviewControl, mousePos.X - WorldPolygonPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPolygonPointPreviewControl, mousePos.Y - WorldPolygonPointPreviewControl.ActualHeight / 2);
        }

        private void WorldPolygonPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var worldPolygon = new WorldPolygon(Map, NamedMapObject.GenerateName("WorldPolygon", Map.WorldPolygons));
            var point = new WorldPolygonPoint(worldPolygon, Canvas.GetLeft(WorldPolygonPreviewControl) + WorldPolygonPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPreviewControl) - WorldPolygonPreviewControl.ActualHeight / 2);
            worldPolygon.Points.Add(point);
            Map.WorldPolygons.Add(worldPolygon);
            ClearWorldPolygonSelection();
            AddWorldPolygonToSelection(worldPolygon);
            MakeWorldPolygonPointLastSelected(point);
            AddWorldPolygonPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NewWorldPolygonRadioButton.IsChecked = false;
        }

        private void WorldPolygonPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                var point = new WorldPolygonPoint(SelectedWorldPolygon, Canvas.GetLeft(WorldPolygonPointPreviewControl) + WorldPolygonPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPointPreviewControl) - WorldPolygonPointPreviewControl.ActualHeight / 2);
                AddWorldPolygonPointToSelection(point, false);
                MakeWorldPolygonPointLastSelected(point);
                SelectedWorldPolygon.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPolygonPointRadioButton.IsChecked = false;
        }

        private void WorldPolygonVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldPolygons)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void WorldPolygonVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldPolygons)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region WorldPointSet

        private void WorldPointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPointSetElements();
        }

        private void WorldPointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPointSetElements();
        }

        private void ShowWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPointSetItemsControl, 1);
            WorldPointSetItemsControl.Opacity = 1;
            WorldPointSetItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPointCheckBox_Unchecked;
            DeleteButton.Click += DeleteWorldPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPoint();
            RotateCheckBox.Checked += RotateWorldPointCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldPointCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldPoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldPointPreview_MouseMove;
            SelectedElement = (UIElement)WorldPointSetItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPointSet);
        }

        private void HideWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPointSetItemsControl, 0);
            WorldPointSetItemsControl.Opacity = 0.5;
            WorldPointSetItemsControl.IsEnabled = false;
            AddWorldPointSetRadioButton.IsChecked = AddWorldPointSetPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPoint();
            RotateCheckBox.Checked -= RotateWorldPointCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotateWorldPointCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotateWorldPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPointButton_Click;
        }

        private void OnWorldPointSetClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPointSet clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPointSet(clickedObject);
                }
                else
                {
                    ClearWorldPointSetSelection();
                    AddWorldPointSetToSelection(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWorldPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPoint(clickedObject);
                }
                else
                {
                    ClearWorldPointSetSelection();
                    AddWorldPointToSelection(clickedObject, false);
                    MakeWorldPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldPointSetSelection();
        }

        private void CtrlSelectWorldPointSet(WorldPointSet worldPolygon)
        {
            if (worldPolygon.IsSelected)
            {
                if (worldPolygon.IsLastSelected)
                {
                    RemoveWorldPointSetFromSelection(worldPolygon);
                }
                else
                {
                    MakeWorldPointSetLastSelected(worldPolygon);
                }
            }
            else
            {
                AddWorldPointSetToSelection(worldPolygon); //Make last selected from ObservableCollection on CollectionChanged
            }
        }

        private void RemoveWorldPointSetFromSelection(WorldPointSet worldPolygon)
        {
            SelectedWorldPointSets.Remove(worldPolygon);
        }

        private void UnselectWorldPointSet(WorldPointSet worldPolygon)
        {
            worldPolygon.IsSelected = false;
            foreach (var p in worldPolygon.Points)
            {
                RemoveWorldPointFromSelection(p);
            }
        }

        private void MakeWorldPointSetLastSelected(WorldPointSet worldPolygon)
        {
            if (SelectedWorldPointSet != null)
            {
                SelectedWorldPointSet.IsLastSelected = false;
            }
            SelectedWorldPointSet = worldPolygon;
            if (SelectedWorldPointSet != null)
            {
                SelectedWorldPointSet.IsLastSelected = true;
                SelectedElement = (UIElement)WorldPointSetItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPointSet);
            }
        }

        private void AddWorldPointSetToSelection(WorldPointSet worldPolygon)
        {
            if (!worldPolygon.IsSelected)
                SelectedWorldPointSets.Add(worldPolygon);
        }

        private void AddWorldPointSetNoPointsToSelection(WorldPointSet worldPolygon)
        {
            if (!worldPolygon.IsSelected)
            {
                var selectedPoints = worldPolygon.Points.Where((p) => p.IsSelected == true).ToArray(); //Save current selection
                SelectedWorldPointSets.Add(worldPolygon); //Triggers selection on all points
                foreach (var p in worldPolygon.Points)
                {
                    if (p.IsSelected && !selectedPoints.Contains(p))
                    {
                        RemoveWorldPointFromSelection(p);
                    }
                    else if (!p.IsSelected && selectedPoints.Contains(p))
                    {
                        AddWorldPointToSelection(p);
                    }
                }
            }
        }

        private void SelectWorldPointSet(WorldPointSet worldPolygon)
        {
            worldPolygon.IsSelected = true;
            foreach (var p in worldPolygon.Points)
            {
                AddWorldPointToSelection(p);
            }
        }

        private void AddWorldPointToSelection(WorldPoint worldPolygonPoint, bool selectOtherPathPoints = true)
        {
            if (!worldPolygonPoint.IsSelected)
            {
                SelectWorldPoint(worldPolygonPoint);
                SelectedWorldPoints.Add(worldPolygonPoint);
                if (selectOtherPathPoints)
                    AddWorldPointSetToSelection(worldPolygonPoint.Parent);
                else
                    AddWorldPointSetNoPointsToSelection(worldPolygonPoint.Parent);
            }
        }

        private void SelectWorldPoint(WorldPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = true;
        }

        private void RemoveWorldPointFromSelection(WorldPoint worldPolygonPoint)
        {
            UnselectWorldPoint(worldPolygonPoint);
            SelectedWorldPoints.Remove(worldPolygonPoint);
            if (!HasWorldPointSetOneSelectedPoint(worldPolygonPoint.Parent))
                RemoveWorldPointSetFromSelection(worldPolygonPoint.Parent);
        }

        private bool HasWorldPointSetOneSelectedPoint(WorldPointSet worldPolygon)
        {
            foreach (var p in worldPolygon.Points)
            {
                if (p.IsSelected) return true;
            }
            return false;
        }

        private void UnselectWorldPoint(WorldPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = false;
            if (worldPolygonPoint.IsLastSelected)
            {
                worldPolygonPoint.IsLastSelected = false;
                SelectedWorldPoint = null;
            }
        }

        private void CtrlSelectWorldPoint(WorldPoint worldPolygonPoint)
        {
            if (worldPolygonPoint.IsSelected)
            {
                if (worldPolygonPoint.IsLastSelected)
                {
                    RemoveWorldPointFromSelection(worldPolygonPoint);
                }
                else
                {
                    MakeWorldPointLastSelected(worldPolygonPoint);
                }
            }
            else
            {
                AddWorldPointToSelection(worldPolygonPoint, false);
                MakeWorldPointLastSelected(worldPolygonPoint);
            }
        }

        private void MakeWorldPointLastSelected(WorldPoint worldPolygonPoint)
        {
            if (SelectedWorldPoint != null)
            {
                SelectedWorldPoint.IsLastSelected = false;
            }
            worldPolygonPoint.IsLastSelected = true;
            SelectedWorldPoint = worldPolygonPoint;
            MakeWorldPointSetLastSelected(worldPolygonPoint.Parent);
        }

        private void ClearWorldPointSetSelection()
        {
            foreach (var v in SelectedWorldPointSets)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPointSets.Clear();
            SelectedWorldPointSet = null;
            SelectedElement = null;
            ClearWorldPointSelection();
        }

        private void ClearWorldPointSelection()
        {
            foreach (var v in SelectedWorldPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPoints.Clear();
            SelectedWorldPoint = null;
        }

        private void MoveWorldPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldPoint();
        }

        private void MoveWorldPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldPoint();
        }

        private void EnableMoveWorldPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldPoints.Count; i++)
            {
                var selectedObject = SelectedWorldPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedWorldPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPointSets.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWorldPointSet = null;
                        SelectedElement = null;
                    }
                }
            }
            SelectedWorldPoints.Clear();
            SelectedWorldPoint = null;
        }

        private void MapGridOutsideWorldPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldPointSetPreviewControl, mousePos.X - WorldPointSetPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPointSetPreviewControl, mousePos.Y - WorldPointSetPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WorldPointPreviewControl, mousePos.X - WorldPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPointPreviewControl, mousePos.Y - WorldPointPreviewControl.ActualHeight / 2);
        }

        private void WorldPointSetPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var worldPolygon = new WorldPointSet(Map, NamedMapObject.GenerateName("WorldPointSet", Map.WorldPointSets));
            var point = new WorldPoint(worldPolygon, Canvas.GetLeft(WorldPointSetPreviewControl) + WorldPointSetPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointSetPreviewControl) - WorldPointSetPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
            worldPolygon.Points.Add(point);
            Map.WorldPointSets.Add(worldPolygon);
            ClearWorldPointSetSelection();
            AddWorldPointSetToSelection(worldPolygon);
            MakeWorldPointLastSelected(point);
            AddWorldPointSetPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointSetPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            AddWorldPointSetRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWorldPointSet != null)
            {
                var point = new WorldPoint(SelectedWorldPointSet, Canvas.GetLeft(WorldPointPreviewControl) + WorldPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointPreviewControl) - WorldPointPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
                AddWorldPointToSelection(point, false);
                MakeWorldPointLastSelected(point);
                SelectedWorldPointSet.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPointSetPointRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = WorldPointSliderRotate.Value + (e.Delta > 0 ? step : -step);
                WorldPointSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
        }

        private void RotateWorldPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotateWorldPoint();
        }

        private void RotateWorldPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotateWorldPoint();
        }

        private void EnableRotateWorldPoint()
        {
            MapGridOutside.MouseWheel += MapGridOutsideWorldPoint_MouseWheel;
        }

        private void DisableRotateWorldPoint()
        {
            MapGridOutside.MouseWheel -= MapGridOutsideWorldPoint_MouseWheel;
        }

        private void MapGridOutsideWorldPoint_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedWorldPoints.Count; i++)
                {
                    var worldPoint = SelectedWorldPoints[i];
                    var newRotation = worldPoint.ZRotation + (e.Delta > 0 ? step : -step);
                    worldPoint.ZRotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        private void WorldPointSetVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldPointSets)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void WorldPointSetVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.WorldPointSets)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region ObjectivePoint

        private void ObjectivePointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowObjectivePointElements();
        }

        private void ObjectivePointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideObjectivePointElements();
        }

        private void ShowObjectivePointElements()
        {
            ObjectivePointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(ObjectivePointItemsControl, 1);
            ObjectivePointItemsControl.Opacity = 1;
            ObjectivePointItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveObjectivePoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click += DeleteObjectivePointButton_Click;
            SelectedElement = (UIElement)ObjectivePointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedObjectivePoint);
        }

        private void HideObjectivePointElements()
        {
            WotDataGrid.SelectedItem = null;
            ObjectivePointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(ObjectivePointItemsControl, 0);
            ObjectivePointItemsControl.Opacity = 0.5;
            ObjectivePointItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveObjectivePoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click -= DeleteObjectivePointButton_Click;
        }

        private void MapGridOutsideObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearObjectivePointSelection();
        }

        private void OnObjectivePointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is ObjectivePoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectObjectivePoint(clickedObject);
                }
                else
                {
                    ClearObjectivePointSelection();
                    SelectAndMakeObjectivePointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void CtrlSelectObjectivePoint(ObjectivePoint objectivePoint)
        {
            if (objectivePoint.IsLastSelected)
            {
                RemoveObjectivePointFromSelection(objectivePoint);
            }
            else
            {
                SelectAndMakeObjectivePointLastSelected(objectivePoint);
            }
        }

        private void SelectAndMakeObjectivePointLastSelected(ObjectivePoint objectivePoint)
        {
            AddObjectivePointToSelection(objectivePoint);
            MakeObjectivePointLastSelected(objectivePoint);
        }

        private void SelectObjectivePoint(ObjectivePoint objectivePoint)
        {
            objectivePoint.IsSelected = true;
        }

        private void AddObjectivePointToSelection(ObjectivePoint objectivePoint)
        {
            if (!objectivePoint.IsSelected)
            {
                objectivePoint.IsSelected = true;
                SelectedObjectivePoints.Add(objectivePoint);
            }
        }

        private void MakeObjectivePointLastSelected(ObjectivePoint objectivePoint)
        {
            if (SelectedObjectivePoint != null)
            {
                SelectedObjectivePoint.IsLastSelected = false;
            }
            SelectedObjectivePoint = objectivePoint;
            if (SelectedObjectivePoint != null)
            {
                SelectedObjectivePoint.IsLastSelected = true;
                SelectedElement = (UIElement)ObjectivePointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedObjectivePoint);
            }
        }

        private void UnselectObjectivePoint(ObjectivePoint objectivePoint)
        {
            objectivePoint.IsSelected = false;
        }

        private void RemoveObjectivePointFromSelection(ObjectivePoint objectivePoint)
        {
            UnselectObjectivePoint(objectivePoint);
            SelectedObjectivePoints.Remove(objectivePoint);
        }

        private void ClearObjectivePointSelection()
        {
            foreach (var v in SelectedObjectivePoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedObjectivePoints.Clear();
            SelectedObjectivePoint = null;
            SelectedElement = null;
        }

        private void MoveObjectivePointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveObjectivePoint();
        }

        private void MoveObjectivePointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveObjectivePoint();
        }

        private void EnableMoveObjectivePoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveObjectivePoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveObjectivePoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveObjectivePoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveObjectivePoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedObjectivePoints.Count; i++)
            {
                var selectedObject = SelectedObjectivePoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteObjectivePointButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedObjectivePoints.ToArray();
            foreach (var item in selectedItems)
            {
                Map.ObjectivePoints.Remove(item);
            }
            SelectedObjectivePoints.Clear();
            SelectedObjectivePoint = null;
            SelectedElement = null;
        }

        private void MapGridOutsideObjectivePointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(ObjectivePointPreviewControl, mousePos.X - ObjectivePointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(ObjectivePointPreviewControl, mousePos.Y - ObjectivePointPreviewControl.ActualHeight / 2);
        }

        private void ObjectivePointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var wot = new ObjectivePoint(Map, NamedMapObject.GenerateName("ObjectivePoint", Map.ObjectivePoints), Canvas.GetLeft(ObjectivePointPreviewControl) + ObjectivePointPreviewControl.ActualWidth / 2, -Canvas.GetTop(ObjectivePointPreviewControl) - ObjectivePointPreviewControl.ActualHeight / 2);
            Map.ObjectivePoints.Add(wot);
            SelectAndMakeObjectivePointLastSelected(wot);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void ObjectivePointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddObjectivePointCheckBox.IsChecked = false;
        }

        private void ObjectivePointVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.ObjectivePoints)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void ObjectivePointVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.ObjectivePoints)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion

        #region MapTextPoint

        private void MapTextPointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowMapTextPointElements();
        }

        private void MapTextPointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideMapTextPointElements();
        }

        private void ShowMapTextPointElements()
        {
            MapTextPointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(MapTextPointItemsControl, 1);
            MapTextPointItemsControl.Opacity = 1;
            MapTextPointItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveMapTextPoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click += DeleteMapTextPointButton_Click;
            SelectedElement = (UIElement)MapTextPointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedMapTextPoint);
        }

        private void HideMapTextPointElements()
        {
            WotDataGrid.SelectedItem = null;
            MapTextPointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(MapTextPointItemsControl, 0);
            MapTextPointItemsControl.Opacity = 0.5;
            MapTextPointItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveMapTextPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click -= DeleteMapTextPointButton_Click;
        }

        private void MapGridOutsideMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMapTextPointSelection();
        }

        private void OnMapTextPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is MapTextPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectMapTextPoint(clickedObject);
                }
                else
                {
                    ClearMapTextPointSelection();
                    SelectAndMakeMapTextPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void CtrlSelectMapTextPoint(MapTextPoint objectivePoint)
        {
            if (objectivePoint.IsLastSelected)
            {
                RemoveMapTextPointFromSelection(objectivePoint);
            }
            else
            {
                SelectAndMakeMapTextPointLastSelected(objectivePoint);
            }
        }

        private void SelectAndMakeMapTextPointLastSelected(MapTextPoint objectivePoint)
        {
            AddMapTextPointToSelection(objectivePoint);
            MakeMapTextPointLastSelected(objectivePoint);
        }

        private void SelectMapTextPoint(MapTextPoint objectivePoint)
        {
            objectivePoint.IsSelected = true;
        }

        private void AddMapTextPointToSelection(MapTextPoint objectivePoint)
        {
            if (!objectivePoint.IsSelected)
            {
                objectivePoint.IsSelected = true;
                SelectedMapTextPoints.Add(objectivePoint);
            }
        }

        private void MakeMapTextPointLastSelected(MapTextPoint objectivePoint)
        {
            if (SelectedMapTextPoint != null)
            {
                SelectedMapTextPoint.IsLastSelected = false;
            }
            SelectedMapTextPoint = objectivePoint;
            if (SelectedMapTextPoint != null)
            {
                SelectedMapTextPoint.IsLastSelected = true;
                SelectedElement = (UIElement)MapTextPointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedMapTextPoint);
            }
        }

        private void UnselectMapTextPoint(MapTextPoint objectivePoint)
        {
            objectivePoint.IsSelected = false;
        }

        private void RemoveMapTextPointFromSelection(MapTextPoint objectivePoint)
        {
            UnselectMapTextPoint(objectivePoint);
            SelectedMapTextPoints.Remove(objectivePoint);
        }

        private void ClearMapTextPointSelection()
        {
            foreach (var v in SelectedMapTextPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedMapTextPoints.Clear();
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void MoveMapTextPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveMapTextPoint();
        }

        private void MoveMapTextPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveMapTextPoint();
        }

        private void EnableMoveMapTextPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveMapTextPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveMapTextPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveMapTextPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveMapTextPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedMapTextPoints.Count; i++)
            {
                var selectedObject = SelectedMapTextPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteMapTextPointButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SelectedMapTextPoints.ToArray();
            foreach (var item in selectedItems)
            {
                Map.MapTextPoints.Remove(item);
            }
            SelectedMapTextPoints.Clear();
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void MapGridOutsideMapTextPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(MapTextPointPreviewControl, mousePos.X - MapTextPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(MapTextPointPreviewControl, mousePos.Y - MapTextPointPreviewControl.ActualHeight / 2);
        }

        private void MapTextPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var text = StringDictionnary.MapTextItems.Keys.ElementAt(MapTextPointPreviewTextComboBox.SelectedIndex);
            var point = new MapTextPoint(Map, NamedMapObject.GenerateName("MapTextPoint", Map.MapTextPoints), text, Canvas.GetLeft(MapTextPointPreviewControl) + MapTextPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(MapTextPointPreviewControl) - MapTextPointPreviewControl.ActualHeight / 2);
            SelectAndMakeMapTextPointLastSelected(point);
            Map.MapTextPoints.Add(point);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void MapTextPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddMapTextPointCheckBox.IsChecked = false;
        }

        private void MapTextPointVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.MapTextPoints)
            {
                obj.IsShownOnUi = true;
            }
        }

        private void MapTextPointVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var obj in Map.MapTextPoints)
            {
                obj.IsShownOnUi = false;
            }
        }

        #endregion
    }
}
