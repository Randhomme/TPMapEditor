using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Enums;
using TPMapEditor.Data.Rule;
using TPMapEditor.Dialogs;
using TPMapEditor.Settings;
using TPMapEditor.Services;
using TPMapEditor.Services.Implementations;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.ViewModel.SelectionTransform;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using TPMapEditor.ViewModel;

namespace TPMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        [ObservableProperty]
        private WorldObjectType? selectedWorldObjectType;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InverseZoom))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessSmall))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessMed))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessLarge))]
        private double zoom = 1;
        private Point selectActionPoint;
        private Point moveActionPoint;
        private DateTime lastWheelTime = DateTime.MinValue;
        private AppSettings settings;
        private Canvas? currentCanvas;

        public double InverseZoom { get => 1 / Zoom; }
        public double ZoomedBorderThicknessSmall { get => 2.5 / Zoom; }
        public double ZoomedBorderThicknessMed { get => 5 / Zoom; }
        public double ZoomedBorderThicknessLarge { get => 10 / Zoom; }

        private readonly IReadOnlyList<(Key key, ModifierKeys modifiers, ICommand command)> keyboardShortcut;

        private readonly ISelectionKBShortcutService worldObjectSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService playerSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService waypointPathSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService worldPolygonSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService worldPointSetSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService objectivePointSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService mapTextPointSelectionKBShortcutService;
        private readonly ICopyPasteService copyPasteService;
        private readonly IUndoManagerService undoManagerService = new UndoManagerService(10);
        public ISelectionService<WorldObject> WorldObjectSelectionService { get; } = new SelectionService<WorldObject>();
        public ISelectionService<Player> PlayerSelectionService { get; } = new SelectionService<Player>();
        public IMultiPointMapObjectSelectionService<WaypointPath, WaypointPathPoint> WaypointPathSelectionService { get; }
        public ISelectionService<WaypointPathPoint> WaypointPathPointSelectionService { get; } = new SelectionService<WaypointPathPoint>();
        public IMultiPointMapObjectSelectionService<WorldPolygon, WorldPolygonPoint> WorldPolygonSelectionService { get; }
        public ISelectionService<WorldPolygonPoint> WorldPolygonPointSelectionService { get; } = new SelectionService<WorldPolygonPoint>();
        public IMultiPointMapObjectSelectionService<WorldPointSet, WorldPoint> WorldPointSetSelectionService { get; }
        public ISelectionService<WorldPoint> WorldPointSelectionService { get; } = new SelectionService<WorldPoint>();
        public ISelectionService<ObjectivePoint> ObjectivePointSelectionService { get; } = new SelectionService<ObjectivePoint>();
        public ISelectionService<MapTextPoint> MapTextPointSelectionService { get; } = new SelectionService<MapTextPoint>();

        public ICollectionView SelectableWorldObjectTypes { get; }

        private ISelectionKBShortcutService currenSelectionKBShortcutService;
        private IEnumerable<IMovableMapObject> currentMovableSelection;

        public WorldMap Map { get; private set; }

        public MainWindow()
        {
            SelectableWorldObjectTypes = new CollectionViewSource() { Source = WorldObjectType.WotTypes }.View;
            SelectableWorldObjectTypes.Filter = WorldObjectType.IsSelectableWorldObjectType;
            settings = new AppSettings();
            Map = new WorldMap();
            copyPasteService = new CopyPasteService();
            WaypointPathSelectionService = new MultiPointMapObjectSelectionService<WaypointPath, WaypointPathPoint>(WaypointPathPointSelectionService);
            WorldPolygonSelectionService = new MultiPointMapObjectSelectionService<WorldPolygon, WorldPolygonPoint>(WorldPolygonPointSelectionService);
            WorldPointSetSelectionService = new MultiPointMapObjectSelectionService<WorldPointSet, WorldPoint>(WorldPointSelectionService);
            worldObjectSelectionKBShortcutService = new SelectionKBShortcutService<WorldObject>(Map.WorldObjects, WorldObjectSelectionService, copyPasteService);
            playerSelectionKBShortcutService = new SelectionKBShortcutService<Player>(Map.Players, PlayerSelectionService, copyPasteService);
            waypointPathSelectionKBShortcutService = new SelectionKBShortcutService<WaypointPath>(Map.WaypointPaths, WaypointPathSelectionService, copyPasteService);
            worldPolygonSelectionKBShortcutService = new SelectionKBShortcutService<WorldPolygon>(Map.WorldPolygons, WorldPolygonSelectionService, copyPasteService);
            worldPointSetSelectionKBShortcutService = new SelectionKBShortcutService<WorldPointSet>(Map.WorldPointSets, WorldPointSetSelectionService, copyPasteService);
            objectivePointSelectionKBShortcutService = new SelectionKBShortcutService<ObjectivePoint>(Map.ObjectivePoints, ObjectivePointSelectionService, copyPasteService);
            mapTextPointSelectionKBShortcutService = new SelectionKBShortcutService<MapTextPoint>(Map.MapTextPoints, MapTextPointSelectionService, copyPasteService);
            undoManagerService.PropertyChanged += UndoManagerService_PropertyChanged;
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"{Title} v{version.Major}.{version.Minor}.{version.Build}";
            currenSelectionKBShortcutService = worldObjectSelectionKBShortcutService;
            currentMovableSelection = WorldObjectSelectionService.SelectedMapObjects;
            keyboardShortcut = new List<(Key key, ModifierKeys modifiers, ICommand command)>()
            {
                (Key.H, ModifierKeys.None, HKeyCommand),
                (Key.H, ModifierKeys.Shift, ShiftHKeyCommand),
                (Key.H, ModifierKeys.Control, CtrlHKeyCommand),
                (Key.A, ModifierKeys.None, AKeyCommand),
                (Key.A, ModifierKeys.Shift, ShiftAKeyCommand),
                (Key.A, ModifierKeys.Control, CtrlAKeyCommand),
                (Key.C, ModifierKeys.Control, CtrlCKeyCommand),
                (Key.V, ModifierKeys.Control, CtrlVKeyCommand),
                (Key.Z, ModifierKeys.Control, UndoCommand),
                (Key.Z, ModifierKeys.Control | ModifierKeys.Shift, RedoCommand),
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
                if (MessageBox.Show("The current map will be cleared. Continue ?", "Map import", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
                    try
                    {
                        ValidateMap(progressLogs);
                        using (var de = new DataExport(sfd.FileName, Map, progressLogs, progress))
                        {
                            de.CreateMapFileAndWriteData();
                        }
                    }
                    catch(Exception ex)
                    {
                        progress.Report("Map export failed.");
                        progressLogs.Report($"An error has occured.\n{ex.Message}");
                    }
                });
            }
        }

        [RelayCommand]
        private void OnStarmapExport()
        {
            new StarmapExportDialog(this, "Starmap preview") { DataContext = new StarmapExportViewModel(Map) }.ShowDialog();
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
            if (MapTextPointSelectionService.SelectedMapObject != null)
            {
                if (!Map.MapTextPoints.Contains(MapTextPointSelectionService.SelectedMapObject))
                {
                    MapTextPointSelectionService.RemoveFromSelection(MapTextPointSelectionService.SelectedMapObject);
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
            if (ObjectivePointSelectionService.SelectedMapObject != null)
            {
                if (!Map.ObjectivePoints.Contains(ObjectivePointSelectionService.SelectedMapObject))
                {
                    ObjectivePointSelectionService.RemoveFromSelection(ObjectivePointSelectionService.SelectedMapObject);
                }
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new CollectionEditorDialog(this, "Players", Map.Players, () => new Player(Map)).ShowDialog();
            if (PlayerSelectionService.SelectedMapObject != null)
            {
                if (!Map.Players.Contains(PlayerSelectionService.SelectedMapObject))
                {
                    PlayerSelectionService.RemoveFromSelection(PlayerSelectionService.SelectedMapObject);
                }
            }
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            new PlayerAllianceDialog(this, "Player alliances", Map).ShowDialog();
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
            if (WaypointPathSelectionService.SelectedMapObject != null)
            {
                if (!Map.WaypointPaths.Contains(WaypointPathSelectionService.SelectedMapObject))
                {
                    WaypointPathSelectionService.RemoveFromSelection(WaypointPathSelectionService.SelectedMapObject);
                    if (WaypointPathPointSelectionService.SelectedMapObject != null)
                        WaypointPathPointSelectionService.RemoveFromSelection(WaypointPathPointSelectionService.SelectedMapObject);
                }
                else if (WaypointPathPointSelectionService.SelectedMapObject != null && !WaypointPathSelectionService.SelectedMapObject.Points.Contains(WaypointPathPointSelectionService.SelectedMapObject))
                {
                    WaypointPathPointSelectionService.RemoveFromSelection(WaypointPathPointSelectionService.SelectedMapObject);
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
            if (WorldObjectSelectionService.SelectedMapObject != null)
            {
                if (!Map.WorldObjects.Contains(WorldObjectSelectionService.SelectedMapObject))
                {
                    //WorldObjectSelectionService.SelectedMapObject = null;
                    //if (WorldObjectRadioButton.IsChecked == true)
                    //    SelectedElement = null;
                    WorldObjectSelectionService.RemoveFromSelection(WorldObjectSelectionService.SelectedMapObject);
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
            if (WorldPointSetSelectionService.SelectedMapObject != null)
            {
                if (!Map.WorldPointSets.Contains(WorldPointSetSelectionService.SelectedMapObject))
                {
                    WorldPointSetSelectionService.RemoveFromSelection(WorldPointSetSelectionService.SelectedMapObject);
                    if (WorldPointSelectionService.SelectedMapObject != null)
                        WorldPointSelectionService.RemoveFromSelection(WorldPointSelectionService.SelectedMapObject);
                }
                else if(WorldPointSelectionService.SelectedMapObject != null && !WorldPointSetSelectionService.SelectedMapObject.Points.Contains(WorldPointSelectionService.SelectedMapObject))
                {
                    WorldPointSelectionService.RemoveFromSelection(WorldPointSelectionService.SelectedMapObject);
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
            if (WorldPolygonSelectionService.SelectedMapObject != null)
            {
                if (!Map.WorldPolygons.Contains(WorldPolygonSelectionService.SelectedMapObject))
                {
                    WorldPolygonSelectionService.RemoveFromSelection(WorldPolygonSelectionService.SelectedMapObject);
                    if (WorldPolygonPointSelectionService.SelectedMapObject != null)
                        WorldPolygonPointSelectionService.RemoveFromSelection(WorldPolygonPointSelectionService.SelectedMapObject);
                }
                else if (WorldPolygonPointSelectionService.SelectedMapObject != null && !WorldPolygonSelectionService.SelectedMapObject.Points.Contains(WorldPolygonPointSelectionService.SelectedMapObject))
                {
                    WorldPolygonPointSelectionService.RemoveFromSelection(WorldPolygonPointSelectionService.SelectedMapObject);
                }
            }
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            new CollectionEditorDialog(this, "World rules", Map.WorldRules, () => new WorldRule(Map, NamedMapObject.GenerateName("WorldRule", Map.WorldRules))).ShowDialog();
        }

        [RelayCommand]
        private void OnAlignTransform()
        {
            new SelectionTransformWindow(this, "Align transform", new AlignTransformViewModel(undoManagerService, currentMovableSelection) { Is3D = IsMovableSelection3D() }).Show();
        }

        [RelayCommand]
        private void OnDistributeTransform()
        {
            new SelectionTransformWindow(this, "Distribute transform", new DistributeTransformViewModel(undoManagerService, currentMovableSelection) { Is3D = IsMovableSelection3D() }).Show();
        }

        [RelayCommand]
        private void OnTranslateTransform()
        {
            new SelectionTransformWindow(this, "Move transform", new TranslateTransformViewModel(undoManagerService, currentMovableSelection) { Is3D = IsMovableSelection3D()}).Show();
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void OnUndo()
        {
            undoManagerService.Undo();
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void OnRedo()
        {
            undoManagerService.Redo();
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
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
            ReloadAllSettings("Reload TPGame folder");
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
                    settings.ReloadWorldObjectTypeList(logs);
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnWiki()
        {
            Process.Start("https://github.com/Randhomme/TPMapEditor/wiki");
        }

        [RelayCommand]
        private void OnChangelogs()
        {
            Process.Start("https://github.com/Randhomme/TPMapEditor/releases");
        }

        [RelayCommand]
        private void OnAboutAppShow()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"TPMapEditor version {v.Major}.{v.Minor}.{v.Build}\nAuthor : Randhomme", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region MainUICommands

        [RelayCommand]
        private void IncreaseZIndex(ISelectableMapObject mapObject)
        {
            mapObject.ZIndex++;
        }

        [RelayCommand]
        private void DecreaseZIndex(ISelectableMapObject mapObject)
        {
            mapObject.ZIndex--;
        }

        #endregion

        #region KeyboardShortcutCommands

        [RelayCommand]
        private void OnHKey()
        {
            currenSelectionKBShortcutService?.OnHKey();
        }

        [RelayCommand]
        private void OnShiftHKey()
        {
            currenSelectionKBShortcutService?.OnShiftHKey();
        }

        [RelayCommand]
        private void OnCtrlHKey()
        {
            currenSelectionKBShortcutService?.OnCtrlHKey();
        }

        [RelayCommand]
        private void OnAKey()
        {
            currenSelectionKBShortcutService?.OnAKey();
        }

        [RelayCommand]
        private void OnShiftAKey()
        {
            currenSelectionKBShortcutService?.OnShiftAKey();
        }

        [RelayCommand]
        private void OnCtrlAKey()
        {
            currenSelectionKBShortcutService?.OnCtrlAKey();
        }

        [RelayCommand]
        private void OnCtrlCKey()
        {
            currenSelectionKBShortcutService.OnCtrlC();
        }

        [RelayCommand]
        private void OnCtrlVKey()
        {
            currenSelectionKBShortcutService.OnCtrlV();
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

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            settings = settings.Load();
            if (string.IsNullOrEmpty(settings.TpGamePath))
            {
                MessageBox.Show("You should set the TPGame path in the application settings before using the map editor.", "TPGame Path Not Set", MessageBoxButton.OK, MessageBoxImage.Warning);
                OnAppSettingsEdit();
            }
            else
            {
                ReloadAllSettings("Load TPGame folder", false);
            }
            Map.Reset();

            #if !DEBUG //Don't check for updates in debug mode
            var local = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            var latest = await GetLatestGitHubVersionAsync();
            if (latest != null && latest > local)
            {
                if (MessageBox.Show(
                    $"A new update is available (v{latest}).\n" +
                    "Do you want to check it out ?",
                    "Update available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                ) == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/Randhomme/TPMapEditor/releases/latest",
                        UseShellExecute = true
                    });
                }
            }
            #endif
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
            double newAbsoluteX = absoluteX * scale;
            double newAbsoluteY = absoluteY * scale;
            double targetX = newAbsoluteX - mousePos.X;
            double targetY = newAbsoluteY - mousePos.Y;
            Zoom = newScale;
            MapScrollViewer.ScrollToHorizontalOffset(targetX);
            MapScrollViewer.ScrollToVerticalOffset(targetY);

            e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Disable default Alt behaviour to allow rotation
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                (RotateCheckBox.IsChecked == true ||
                (WorldObjectPreviewControl.Visibility == Visibility.Visible) ||
                (PlayerPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointSetPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointPreviewControl.Visibility == Visibility.Visible)))
                e.Handled = true;

            //Keyboard shortcuts
            var command = GetKBShortcutCommand(e.Key, Keyboard.Modifiers);
            if(command!=null && command.CanExecute(null) && !IsTextInputActive())
            {
                command.Execute(null);
                e.Handled = true;
            }
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

        private void MapScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            MapScrollViewer.Focus();
        }

        private void UndoManagerService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IUndoManagerService.CanUndo))
            {
                UndoCommand.NotifyCanExecuteChanged();
                RedoCommand.NotifyCanExecuteChanged();
            }

            if (e.PropertyName == nameof(IUndoManagerService.CanRedo))
            {
                UndoCommand.NotifyCanExecuteChanged();
                RedoCommand.NotifyCanExecuteChanged();
            }
        }

        private void MapGridOutsideSelect_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(PreviewCanvas);
            if (selectActionPoint.X < pos.X)
            {
                Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
                SelectionRectangle.Width = pos.X - selectActionPoint.X;
            }
            else
            {
                Canvas.SetLeft(SelectionRectangle, pos.X);
                SelectionRectangle.Width = selectActionPoint.X - pos.X;
            }

            if (selectActionPoint.Y < pos.Y)
            {
                Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
                SelectionRectangle.Height = pos.Y - selectActionPoint.Y;
            }
            else
            {
                Canvas.SetTop(SelectionRectangle, pos.Y);
                SelectionRectangle.Height = selectActionPoint.Y - pos.Y;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HidePlayerElements();
            HideWaypointPathElements();
            HideWorldPolygonElements();
            HideWorldPointSetElements();
            HideObjectivePointElements();
            HideMapTextPointElements();
            WorldObjectRadioButton.IsChecked = true;
        }

        #endregion

        #region UtilsMethods

        /// <summary>
        /// Check for a new release
        /// </summary>
        /// <returns></returns>
        private async Task<Version?> GetLatestGitHubVersionAsync()
        {
            try
            {
                using var http = new HttpClient();

                // GitHub demande obligatoirement un User-Agent
                http.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp");

                var url = "https://api.github.com/repos/Randhomme/TPMapEditor/releases/latest";

                var json = await http.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);

                // Récupère "tag_name" => "v1.4.2"
                var tag = doc.RootElement.GetProperty("tag_name").GetString();

                if (tag == null)
                    return null;

                // Supprime le "v"
                tag = tag.TrimStart('v');

                return Version.TryParse(tag, out var version) ? version : null;
            }
            catch { return null; }
        }

        /// <summary>
        /// Clear all the selections
        /// </summary>
        private void ClearSelections()
        {
            WorldObjectSelectionService.ClearSelection();
            PlayerSelectionService.ClearSelection();
            WaypointPathSelectionService.ClearSelection();
            WaypointPathPointSelectionService.ClearSelection();
            WorldPolygonSelectionService.ClearSelection();
            WorldPolygonPointSelectionService.ClearSelection();
            WorldPointSetSelectionService.ClearSelection();
            WorldPointSelectionService.ClearSelection();
            ObjectivePointSelectionService.ClearSelection();
            MapTextPointSelectionService.ClearSelection();
        }

        /// <summary>
        /// Returns a rotation between -180 and 180
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private double GetRotation(double rotation)
        {
            if (rotation > 180) rotation -= 360;
            else if (rotation < -180) rotation += 360;
            return rotation;
        }

        /// <summary>
        /// Allows a smooth rotation using mouse wheel
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Reload all the app settings
        /// </summary>
        /// <param name="title"></param>
        /// <param name="notifyOnFinish"></param>
        private void ReloadAllSettings(string title, bool notifyOnFinish = true)
        {
            new ProgressDialog(this, title).RunActionSameThread((progress, progressLogs) =>
            {
                progress.Report("Reloading ...");
                settings.ReloadAll(progress, progressLogs);
                progress.Report("Reloading complete");
                MapTextPointPreviewTextComboBox.SelectedIndex = 0;
            }, true, notifyOnFinish);
        }

        /// <summary>
        /// Validate everything in the map
        /// </summary>
        /// <param name="progressLogs"></param>
        public void ValidateMap(IProgress<string> progressLogs)
        {
            Map.ValidateAllProperties();
            for (int i = 0; i < Map.WorldObjects.Count; i++)
            {
                var item = Map.WorldObjects[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldObject '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.SelectableTeams.Count; i++)
            {
                var item = Map.SelectableTeams[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"SelectableTeam '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.InGameTeams.Count; i++)
            {
                var item = Map.InGameTeams[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"InGameTeam '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.Players.Count; i++)
            {
                var item = Map.Players[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"Player '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.Groups.Count; i++)
            {
                var item = Map.Groups[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"Group '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WaypointPaths.Count; i++)
            {
                var item = Map.WaypointPaths[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WaypointPath '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WorldPolygons.Count; i++)
            {
                var item = Map.WorldPolygons[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldPolygon '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WorldPointSets.Count; i++)
            {
                var item = Map.WorldPointSets[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldPointSet '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.Flags.Count; i++)
            {
                var item = Map.Flags[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"Flag '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.PlayerAlliances.Count; i++)
            {
                var item = Map.PlayerAlliances[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"PlayerAlliance {i + 1} is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.Timers.Count; i++)
            {
                var item = Map.Timers[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"Timer '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.SpeechEvents.Count; i++)
            {
                var item = Map.SpeechEvents[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"SpeechEvent {i + 1} is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WorldRules.Count; i++)
            {
                var item = Map.WorldRules[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldRule '{item}' is invalid : {ex.Message}"); }
                foreach (var item1 in item.Conditions)
                {
                    foreach (var item2 in item1.RuleFields)
                    {
                        if (item2 is RuleFieldObservableCollection rfoc && rfoc.Value != null)
                        {
                            foreach (var item3 in rfoc.Value)
                            {
                                try { item3.ValidateAllProperties(); }
                                catch { progressLogs.Report($"Invalid Value for Rule '{item.Name}', Condition '{item1.Type.GetName()}', Field '{item3.RealLabel}'"); }
                            }
                        }
                        else
                        {
                            try { item2.ValidateAllProperties(); }
                            catch { progressLogs.Report($"Invalid Value for Rule '{item.Name}', Condition '{item1.Type.GetName()}', Field '{item2.RealLabel}'"); }
                        }
                    }
                }
                foreach (var item1 in item.Actions)
                {
                    foreach (var item2 in item1.RuleFields)
                    {
                        if (item2 is RuleFieldObservableCollection rfoc && rfoc.Value != null)
                        {
                            foreach (var item3 in rfoc.Value)
                            {
                                try { item3.ValidateAllProperties(); }
                                catch { progressLogs.Report($"Invalid Value for Rule '{item.Name}', Condition '{item1.Type.GetName()}', Field '{item3.RealLabel}'"); }
                            }
                        }
                        else
                        {
                            try { item2.ValidateAllProperties(); }
                            catch { progressLogs.Report($"Invalid Value for Rule '{item.Name}', Condition '{item1.Type.GetName()}', Field '{item2.RealLabel}'"); }
                        }
                    }
                }
            }
            for (int i = 0; i < Map.ShipUnits.Count; i++)
            {
                var item = Map.ShipUnits[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"ShipUnit '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.ObjectivePoints.Count; i++)
            {
                var item = Map.ObjectivePoints[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"ObjectivePoint '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.ObjectiveTasks.Count; i++)
            {
                var item = Map.ObjectiveTasks[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"ObjectiveTask '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.MapTextPoints.Count; i++)
            {
                var item = Map.MapTextPoints[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"MapTextPoint '{item}' is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.JournalEntries.Count; i++)
            {
                var item = Map.JournalEntries[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"JournalEntry {i + 1} is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WorldCrews.Count; i++)
            {
                var item = Map.WorldCrews[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldCrew {i + 1} is invalid : {ex.Message}"); }
            }
            for (int i = 0; i < Map.WorldArms.Count; i++)
            {
                var item = Map.WorldArms[i];
                try { item.ValidateAllProperties(); }
                catch (Exception ex) { progressLogs.Report($"WorldArm {i + 1} is invalid : {ex.Message}"); }
            }
        }

        /// <summary>
        /// CanExecute for UndoCommand
        /// </summary>
        /// <returns></returns>
        private bool CanUndo() => undoManagerService.CanUndo;

        /// <summary>
        /// CanExecute for RedoCommand
        /// </summary>
        /// <returns></returns>
        private bool CanRedo() => undoManagerService.CanRedo;

        /// <summary>
        /// Checks if the movable selection is 3D or not (only WorldPolygons are in 2D)
        /// </summary>
        /// <returns></returns>
        private bool IsMovableSelection3D()
        {
            if (WorldPolygonRadioButton.IsChecked == true)
                return false;
            return true;
        }

        /// <summary>
        /// Execute the keyboard shortcut if it exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns>True if executed, false otherwise</returns>
        private ICommand? GetKBShortcutCommand(Key key, ModifierKeys modifiers)
        {
            foreach (var kbShortcut in keyboardShortcut)
            {
                if(kbShortcut.key == key && kbShortcut.modifiers == modifiers)
                {
                    return kbShortcut.command;
                }
            }
            return null;
        }

        private static bool IsTextInputActive()
        {
            DependencyObject? current = Keyboard.FocusedElement as DependencyObject;

            if (current is TextBoxBase textBoxBase)
                return textBoxBase.IsSelectionActive;

            return false;
        }

        private T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is T tChild)
                    return tChild;

                T? childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldObject();
            RotateCheckBox.Checked += RotateWorldObjectCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldObjectCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldObject();
            MapGridOutside.MouseMove += MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click += DeleteWorldObjectButton_Click;
            currenSelectionKBShortcutService = worldObjectSelectionKBShortcutService;
            currentMovableSelection = WorldObjectSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(WorldObjectItemsControl);
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldObjectButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                WorldObjectSelectionService.ClearSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.WorldObjects.Count; i++)
                {
                    var obj = Map.WorldObjects[i];
                    if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                        WorldObjectSelectionService.SelectAndMakeLastSelected(obj);
                }
                e.Handled = true;
            }
            else if (WorldObjectPreviewControl.Visibility != Visibility.Visible)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnWorldObjectClicked(s, e);
            }
        }

        private void OnWorldObjectClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldObject clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    WorldObjectSelectionService.CtrlSelect(clickedObject);
                }
                else
                {
                    WorldObjectSelectionService.ClearSelection();
                    WorldObjectSelectionService.SelectAndMakeLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldObject()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldObject_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldObject_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldObject_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < WorldObjectSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = WorldObjectSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldObjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = WorldObjectSelectionService.SelectedMapObjects.ToArray();
            foreach(var item in selectedItems)
            {
                Map.WorldObjects.Remove(item);
            }
            WorldObjectSelectionService.ClearSelection();
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
            WorldObjectSelectionService.SelectAndMakeLastSelected(wot);
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
                for (int i = 0; i < WorldObjectSelectionService.SelectedMapObjects.Count; i++)
                {
                    var worldObject = WorldObjectSelectionService.SelectedMapObjects[i];
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked += MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMovePlayer();
            RotateCheckBox.Checked += RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked += RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotatePlayer();
            MapGridOutside.MouseMove += MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click += DeletePlayerButton_Click;
            currenSelectionKBShortcutService = playerSelectionKBShortcutService;
            currentMovableSelection = PlayerSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(PlayerItemsControl);
        }

        private void HidePlayerElements()
        {
            WotDataGrid.SelectedItem = null;
            PlayerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(PlayerItemsControl, 0);
            PlayerItemsControl.Opacity = 0.5;
            PlayerItemsControl.IsEnabled = false;
            AddPlayerCheckBox.IsChecked = false;
            MoveCheckBox.Checked -= MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked -= MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMovePlayer();
            RotateCheckBox.Checked -= RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotatePlayer();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click -= DeletePlayerButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsidePlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                PlayerSelectionService.ClearSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsidePlayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.Players.Count; i++)
                {
                    var obj = Map.Players[i];
                    if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                        PlayerSelectionService.SelectAndMakeLastSelected(obj);
                }
                e.Handled = true;
            }
            else if (AddPlayerCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnPlayerClicked(s, e);
            }
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is Player clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    PlayerSelectionService.CtrlSelect(clickedObject);
                }
                else
                {
                    PlayerSelectionService.ClearSelection();
                    PlayerSelectionService.SelectAndMakeLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMovePlayer()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMovePlayer_MouseMove;
        }

        private void MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMovePlayer_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMovePlayer_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < PlayerSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = PlayerSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = PlayerSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.Players.Remove(item);
            }
            PlayerSelectionService.ClearSelection();
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
            PlayerSelectionService.SelectAndMakeLastSelected(player);
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
                for (int i = 0; i < PlayerSelectionService.SelectedMapObjects.Count; i++)
                {
                    var player = PlayerSelectionService.SelectedMapObjects[i];
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
            MoveCheckBox.Checked += MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWaypointPathPointCheckBox_Unchecked;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove += MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click += DeleteWaypointPathPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWaypointPathPoint();
            currenSelectionKBShortcutService = waypointPathSelectionKBShortcutService;
            currentMovableSelection = WaypointPathPointSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(WaypointPathItemsControl);
        }

        private void HideWaypointPathElements()
        {
            WaypointPathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WaypointPathItemsControl, 0);
            WaypointPathItemsControl.Opacity = 0.5;
            WaypointPathItemsControl.IsEnabled = false;
            NewWaypointPathRadioButton.IsChecked = AddWaypointPathPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWaypointPathPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWaypointPathPoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWaypointPathPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && NewWaypointPathRadioButton.IsChecked == false && AddWaypointPathPointRadioButton.IsChecked == false)
            {
                WaypointPathSelectionService.ClearSelection();
                WaypointPathPointSelectionService.ClearSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.WaypointPaths.Count; i++)
                {
                    var path = Map.WaypointPaths[i];
                    if (path.IsShownOnUi)
                    {
                        for(int j = 0; j < path.Points.Count; j++)
                        {
                            var p = path.Points[j];
                            if (rect.Contains(new Point(p.X, -p.Y)))
                            {
                                WaypointPathSelectionService.SelectAndMakeLastSelectedWithoutPoints(p.Parent);
                                WaypointPathPointSelectionService.SelectAndMakeLastSelected(p);
                            }

                        }
                    }
                }
                e.Handled = true;
            }
            else if (AddWaypointPathPointRadioButton.IsChecked != true && NewWaypointPathRadioButton.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null && SelectCheckBox.IsChecked == true)
                    if (s is FrameworkElement element)
                        if (element.DataContext is WaypointPath path)
                            WaypointPathClicked(path);
                        else if (element.DataContext is WaypointPathPoint point)
                            WaypointPathPointClicked(point);
            }
        }

        private void OnWaypointPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPath clickedObject)
            {
                WaypointPathClicked(clickedObject);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPathPoint clickedObject)
            {
                WaypointPathPointClicked(clickedObject);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void WaypointPathClicked(WaypointPath path)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

            if (ctrlPressed)
            {
                if (path.IsLastSelected)
                {
                    WaypointPathSelectionService.RemoveFromSelection(path);
                }
                else
                {
                    WaypointPathSelectionService.SelectAndMakeLastSelected(path);
                }
            }
            else
            {
                WaypointPathSelectionService.ClearSelection();
                WaypointPathPointSelectionService.ClearSelection();
                WaypointPathSelectionService.SelectAndMakeLastSelected(path);
                foreach (var item in path.Points)
                {
                    WaypointPathPointSelectionService.SelectAndMakeLastSelected(item);
                }
            }
        }

        private void WaypointPathPointClicked(WaypointPathPoint point)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

            if (ctrlPressed)
            {
                if (point.IsLastSelected)
                {
                    WaypointPathPointSelectionService.RemoveFromSelection(point);
                    WaypointPathSelectionService.RemoveFromSelectionWithoutPoints(point.Parent);
                    if (WaypointPathPointSelectionService.SelectedMapObject != null)
                        WaypointPathSelectionService.MakeLastSelected(WaypointPathPointSelectionService.SelectedMapObject.Parent);
                }
                else
                {
                    WaypointPathSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                    WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
                }
            }
            else
            {
                WaypointPathSelectionService.ClearSelection();
                WaypointPathPointSelectionService.ClearSelection();
                WaypointPathSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWaypointPathPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWaypointPathPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWaypointPathPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWaypointPathPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            for (var i = 0; i < WaypointPathPointSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = WaypointPathPointSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in WaypointPathPointSelectionService.SelectedMapObjects)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if(p.Parent.Points.Count == 0)
                {
                    Map.WaypointPaths.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        WaypointPathSelectionService.RemoveFromSelection(p.Parent);
                    }
                }
            }
            WaypointPathSelectionService.ClearSelection();
            WaypointPathPointSelectionService.ClearSelection();
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
            WaypointPathSelectionService.ClearSelection();
            WaypointPathPointSelectionService.ClearSelection();
            WaypointPathSelectionService.SelectAndMakeLastSelected(waypointPath);
            WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
            AddWaypointPathPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NewWaypointPathRadioButton.IsChecked = false;
        }

        private void WaypointPathPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WaypointPathSelectionService.SelectedMapObject != null)
            {
                var point = new WaypointPathPoint(WaypointPathSelectionService.SelectedMapObject, Canvas.GetLeft(WaypointPathPointPreviewControl) + WaypointPathPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WaypointPathPointPreviewControl) - WaypointPathPointPreviewControl.ActualHeight / 2, 0);
                WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
                WaypointPathSelectionService.SelectedMapObject.Points.Add(point);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove += MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click += DeleteWorldPolygonPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPolygonPoint();
            currenSelectionKBShortcutService = worldPolygonSelectionKBShortcutService;
            currentMovableSelection = WorldPolygonPointSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(WorldPolygonItemsControl);
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPolygonPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && NewWorldPolygonRadioButton.IsChecked == false && AddWorldPolygonPointRadioButton.IsChecked == false)
            {
                WorldPolygonSelectionService.ClearSelection();
                WorldPolygonPointSelectionService.ClearSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.WorldPolygons.Count; i++)
                {
                    var path = Map.WorldPolygons[i];
                    if (path.IsShownOnUi)
                    {
                        for (int j = 0; j < path.Points.Count; j++)
                        {
                            var p = path.Points[j];
                            if (rect.Contains(new Point(p.X, -p.Y)))
                            {
                                WorldPolygonSelectionService.SelectAndMakeLastSelectedWithoutPoints(p.Parent);
                                WorldPolygonPointSelectionService.SelectAndMakeLastSelected(p);
                            }

                        }
                    }
                }
                e.Handled = true;
            }
            else if (AddWorldPolygonPointRadioButton.IsChecked != true && NewWorldPolygonRadioButton.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null && SelectCheckBox.IsChecked == true)
                    if (s is FrameworkElement element)
                        if (element.DataContext is WorldPolygon polygon)
                            WorldPolygonClicked(polygon);
                        else if (element.DataContext is WorldPolygonPoint point)
                            WorldPolygonPointClicked(point);
            }
        }

        private void OnWorldPolygonClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygon clickedObject)
            {
                WorldPolygonClicked(clickedObject);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void WorldPolygonClicked(WorldPolygon polygon)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

            if (ctrlPressed)
            {
                if (polygon.IsLastSelected)
                {
                    WorldPolygonSelectionService.RemoveFromSelection(polygon);
                }
                else
                {
                    WorldPolygonSelectionService.SelectAndMakeLastSelected(polygon);
                }
            }
            else
            {
                WorldPolygonSelectionService.ClearSelection();
                WorldPolygonPointSelectionService.ClearSelection();
                WorldPolygonSelectionService.SelectAndMakeLastSelected(polygon);
                foreach (var item in polygon.Points)
                {
                    WorldPolygonPointSelectionService.SelectAndMakeLastSelected(item);
                }
            }
        }

        private void OnWorldPolygonPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygonPoint clickedObject)
            {
                WorldPolygonPointClicked(clickedObject);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void WorldPolygonPointClicked(WorldPolygonPoint point)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

            if (ctrlPressed)
            {
                if (point.IsLastSelected)
                {
                    WorldPolygonPointSelectionService.RemoveFromSelection(point);
                    WorldPolygonSelectionService.RemoveFromSelectionWithoutPoints(point.Parent);
                    if (WorldPolygonPointSelectionService.SelectedMapObject != null)
                        WorldPolygonSelectionService.MakeLastSelected(WorldPolygonPointSelectionService.SelectedMapObject.Parent);
                }
                else
                {
                    WorldPolygonSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                    WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
                }
            }
            else
            {
                WorldPolygonSelectionService.ClearSelection();
                WorldPolygonPointSelectionService.ClearSelection();
                WorldPolygonSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPolygonPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPolygonPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPolygonPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < WorldPolygonPointSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = WorldPolygonPointSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPolygonPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in WorldPolygonPointSelectionService.SelectedMapObjects)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPolygons.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        WorldPolygonSelectionService.RemoveFromSelection(p.Parent);
                    }
                }
            }
            WorldPolygonSelectionService.ClearSelection();
            WorldPolygonPointSelectionService.ClearSelection();
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
            WorldPolygonSelectionService.ClearSelection();
            WorldPolygonPointSelectionService.ClearSelection();
            WorldPolygonSelectionService.SelectAndMakeLastSelected(worldPolygon);
            WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
            AddWorldPolygonPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NewWorldPolygonRadioButton.IsChecked = false;
        }

        private void WorldPolygonPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WorldPolygonSelectionService.SelectedMapObject != null)
            {
                var point = new WorldPolygonPoint(WorldPolygonSelectionService.SelectedMapObject, Canvas.GetLeft(WorldPolygonPointPreviewControl) + WorldPolygonPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPointPreviewControl) - WorldPolygonPointPreviewControl.ActualHeight / 2);
                WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
                WorldPolygonSelectionService.SelectAndMakeLastSelectedWithoutPoints(WorldPolygonSelectionService.SelectedMapObject);
                WorldPolygonSelectionService.SelectedMapObject.Points.Add(point);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPointCheckBox_Unchecked;
            DeleteButton.Click += DeleteWorldPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPoint();
            RotateCheckBox.Checked += RotateWorldPointCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldPointCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldPoint();
            MapGridOutside.MouseMove += MapGridOutsideWorldPointPreview_MouseMove;
            currenSelectionKBShortcutService = worldPointSetSelectionKBShortcutService;
            currentMovableSelection = WorldPointSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(WorldPointSetItemsControl);
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && AddWorldPointSetRadioButton.IsChecked == false && AddWorldPointSetPointRadioButton.IsChecked == false)
            {
                WorldPointSetSelectionService.ClearSelection();
                WorldPointSelectionService.ClearSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.WorldPointSets.Count; i++)
                {
                    var path = Map.WorldPointSets[i];
                    if (path.IsShownOnUi)
                    {
                        for (int j = 0; j < path.Points.Count; j++)
                        {
                            var p = path.Points[j];
                            if (rect.Contains(new Point(p.X, -p.Y)))
                            {
                                WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(p.Parent);
                                WorldPointSelectionService.SelectAndMakeLastSelected(p);
                            }

                        }
                    }
                }
                e.Handled = true;
            }
            else if (AddWorldPointSetPointRadioButton.IsChecked != true && AddWorldPointSetRadioButton.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnWorldPointClicked(s, e);
            }
        }

        private void OnWorldPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    if (clickedObject.IsLastSelected)
                    {
                        WorldPointSelectionService.RemoveFromSelection(clickedObject);
                        WorldPointSetSelectionService.RemoveFromSelectionWithoutPoints(clickedObject.Parent);
                        if (WorldPointSelectionService.SelectedMapObject != null)
                            WorldPointSetSelectionService.MakeLastSelected(WorldPointSelectionService.SelectedMapObject.Parent);
                    }
                    else
                    {
                        WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(clickedObject.Parent);
                        WorldPointSelectionService.SelectAndMakeLastSelected(clickedObject);
                    }
                }
                else
                {
                    WorldPointSetSelectionService.ClearSelection();
                    WorldPointSelectionService.ClearSelection();
                    WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(clickedObject.Parent);
                    WorldPointSelectionService.SelectAndMakeLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < WorldPointSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = WorldPointSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in WorldPointSelectionService.SelectedMapObjects)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPointSets.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        WorldPointSetSelectionService.RemoveFromSelection(p.Parent);
                    }
                }
            }
            WorldPointSetSelectionService.ClearSelection();
            WorldPointSelectionService.ClearSelection();
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
            var worldPointSet = new WorldPointSet(Map, NamedMapObject.GenerateName("WorldPointSet", Map.WorldPointSets));
            var point = new WorldPoint(worldPointSet, Canvas.GetLeft(WorldPointSetPreviewControl) + WorldPointSetPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointSetPreviewControl) - WorldPointSetPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
            worldPointSet.Points.Add(point);
            Map.WorldPointSets.Add(worldPointSet);
            WorldPointSetSelectionService.ClearSelection();
            WorldPointSelectionService.ClearSelection();
            WorldPointSetSelectionService.SelectAndMakeLastSelected(worldPointSet);
            WorldPointSelectionService.SelectAndMakeLastSelected(point);
            AddWorldPointSetPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointSetPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            AddWorldPointSetRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WorldPointSetSelectionService.SelectedMapObject != null)
            {
                var point = new WorldPoint(WorldPointSetSelectionService.SelectedMapObject, Canvas.GetLeft(WorldPointPreviewControl) + WorldPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointPreviewControl) - WorldPointPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
                WorldPointSelectionService.SelectAndMakeLastSelected(point);
                WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(WorldPointSetSelectionService.SelectedMapObject);
                WorldPointSetSelectionService.SelectedMapObject.Points.Add(point);
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
                for (int i = 0; i < WorldPointSelectionService.SelectedMapObjects.Count; i++)
                {
                    var worldPoint = WorldPointSelectionService.SelectedMapObjects[i];
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveObjectivePoint();
            MapGridOutside.MouseMove += MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click += DeleteObjectivePointButton_Click;
            currenSelectionKBShortcutService = objectivePointSelectionKBShortcutService;
            currentMovableSelection = ObjectivePointSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(ObjectivePointItemsControl);
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click -= DeleteObjectivePointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                ObjectivePointSelectionService.ClearSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.ObjectivePoints.Count; i++)
                {
                    var obj = Map.ObjectivePoints[i];
                    if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                        ObjectivePointSelectionService.SelectAndMakeLastSelected(obj);
                }
                e.Handled = true;
            }
            else if(AddObjectivePointCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnObjectivePointClicked(s, e);
            }
        }

        private void OnObjectivePointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is ObjectivePoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    ObjectivePointSelectionService.CtrlSelect(clickedObject);
                }
                else
                {
                    ObjectivePointSelectionService.ClearSelection();
                    ObjectivePointSelectionService.SelectAndMakeLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveObjectivePoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveObjectivePoint_MouseMove;
        }

        private void MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveObjectivePoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveObjectivePoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < ObjectivePointSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = ObjectivePointSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteObjectivePointButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ObjectivePointSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.ObjectivePoints.Remove(item);
            }
            ObjectivePointSelectionService.ClearSelection();
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
            ObjectivePointSelectionService.SelectAndMakeLastSelected(wot);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveMapTextPoint();
            MapGridOutside.MouseMove += MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click += DeleteMapTextPointButton_Click;
            currenSelectionKBShortcutService = mapTextPointSelectionKBShortcutService;
            currentMovableSelection = MapTextPointSelectionService.SelectedMapObjects;
            currentCanvas = FindVisualChild<Canvas>(MapTextPointItemsControl);
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click -= DeleteMapTextPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                MapTextPointSelectionService.ClearSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance * InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance * InverseZoom)
            {
                for (int i = 0; i < Map.MapTextPoints.Count; i++)
                {
                    var obj = Map.MapTextPoints[i];
                    if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                        MapTextPointSelectionService.SelectAndMakeLastSelected(obj);
                }
                e.Handled = true;
            }
            else if (AddMapTextPointCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnObjectivePointClicked(s, e);
            }
        }

        private void OnMapTextPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is MapTextPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    MapTextPointSelectionService.CtrlSelect(clickedObject);
                }
                else
                {
                    MapTextPointSelectionService.ClearSelection();
                    MapTextPointSelectionService.SelectAndMakeLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveMapTextPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveMapTextPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveMapTextPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveMapTextPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < MapTextPointSelectionService.SelectedMapObjects.Count; i++)
            {
                var selectedObject = MapTextPointSelectionService.SelectedMapObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteMapTextPointButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = MapTextPointSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.MapTextPoints.Remove(item);
            }
            MapTextPointSelectionService.ClearSelection();
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
            MapTextPointSelectionService.SelectAndMakeLastSelected(point);
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
