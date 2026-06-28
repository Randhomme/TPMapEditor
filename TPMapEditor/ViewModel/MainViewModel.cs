using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Data.Rule;
using TPMapEditor.Dialogs;
using TPMapEditor.Enums;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;
using TPMapEditor.Services.Implementations;
using TPMapEditor.Settings;

namespace TPMapEditor.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private AppSettings settings;
        private bool shouldCommitTransformMapCommand = false;
        private bool hasCommittedTransformMapCommand = false;

        [ObservableProperty]
        private WorldObjectType? selectedWorldObjectType;
        [ObservableProperty]
        private int selectedMapTextPointPreviewTextIndex = -1;
        [ObservableProperty]
        private string transformMapCommandTitle = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InverseZoom))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessSmall))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessMed))]
        [NotifyPropertyChangedFor(nameof(ZoomedBorderThicknessLarge))]
        private double zoom = 1;

        [ObservableProperty]
        private bool isSelectCommandActive = true, isMoveCommandActive, isRotateCommandActive, canChangeRotationMode, isAlignCommandActive, isDistributeCommandActive;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTransformingMap))]
        private IUndoableMapCommand? currentMapCommand;

        private AlignTransformMapCommand? alignTransformMapCommand;
        private DistributeTransformMapCommand? distributeTransformMapCommand;
        private TranslateTransformMapCommand? translateTransformMapCommand;
        private RotateOrbitSpinTransformMapCommand? rotateOrbitSpinTransformMapCommand;
        private RotateOrbitTransformMapCommand? rotateOrbitTransformMapCommand;

        public bool IsTransformingMap { get => CurrentMapCommand != null; }

        public double InverseZoom { get => 1 / Zoom; }
        public double ZoomedBorderThicknessSmall { get => 2.5 / Zoom; }
        public double ZoomedBorderThicknessMed { get => 5 / Zoom; }
        public double ZoomedBorderThicknessLarge { get => 10 / Zoom; }
        public bool IsMovableSelection3D { get; set; }

        private readonly IReadOnlyList<(Key key, ModifierKeys modifiers, ICommand command)> keyboardShortcut;
        private readonly ISelectionKBShortcutService worldObjectSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService playerSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService waypointPathSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService worldPolygonSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService worldPointSetSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService objectivePointSelectionKBShortcutService;
        private readonly ISelectionKBShortcutService mapTextPointSelectionKBShortcutService;
        private readonly ICopyPasteService copyPasteService;
        private readonly ICopyPasteService ruleConditionCopyPasteService;
        private readonly ICopyPasteService ruleActionCopyPasteService;
        private readonly ICopyPasteService waypointPathPointCopyPasteService;
        private readonly ICopyPasteService worldPolygonPointCopyPasteService;
        private readonly ICopyPasteService worldPointCopyPasteService;
        private readonly IUndoManagerService undoManagerService = new UndoManagerService(20);
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

        public MultiWorldObjectViewModel MultiWorldObjectViewModel { get; }
        public MultiPlayerViewModel MultiPlayerViewModel { get; }
        public MultiWaypointPathViewModel MultiWaypointPathViewModel { get; }
        public MultiWaypointPathPointViewModel MultiWaypointPathPointViewModel { get; }
        public MultiWorldPolygonViewModel MultiWorldPolygonViewModel { get; }
        public MultiWorldPolygonPointViewModel MultiWorldPolygonPointViewModel { get; }
        public MultiWorldPointSetViewModel MultiWorldPointSetViewModel { get; }
        public MultiWorldPointViewModel MultiWorldPointViewModel { get; }
        public MultiObjectivePointViewModel MultiObjectivePointViewModel { get; }
        public MultiMapTextPointViewModel MultiMapTextPointViewModel { get; }

        public ICollectionView SelectableWorldObjectTypes { get; }

        private ISelectionKBShortcutService currenSelectionKBShortcutService;
        private IMultiMovableMapObject currentMultiMovableMapObject;
        private IMultiRotatableMapObject? currentMultiRotatableMapObject;


        public WorldMap Map { get; set; }

        public MainViewModel()
        {
            SelectableWorldObjectTypes = new CollectionViewSource() { Source = WorldObjectType.WotTypes }.View;
            SelectableWorldObjectTypes.Filter = WorldObjectType.IsSelectableWorldObjectType;
            settings = new AppSettings();
            Map = new WorldMap();
            copyPasteService = new CopyPasteService();
            ruleConditionCopyPasteService = new CopyPasteService();
            ruleActionCopyPasteService = new CopyPasteService();
            waypointPathPointCopyPasteService = new CopyPasteService();
            worldPolygonPointCopyPasteService = new CopyPasteService();
            worldPointCopyPasteService = new CopyPasteService();
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
            MultiWorldObjectViewModel = new(WorldObjectSelectionService.SelectedMapObjects, undoManagerService, Map);
            MultiPlayerViewModel = new(PlayerSelectionService.SelectedMapObjects, undoManagerService, Map);
            MultiWaypointPathViewModel = new(WaypointPathSelectionService.SelectedMapObjects, undoManagerService);
            MultiWaypointPathPointViewModel = new(WaypointPathPointSelectionService.SelectedMapObjects, undoManagerService);
            MultiWorldPolygonViewModel = new(WorldPolygonSelectionService.SelectedMapObjects, undoManagerService);
            MultiWorldPolygonPointViewModel = new(WorldPolygonPointSelectionService.SelectedMapObjects, undoManagerService);
            MultiWorldPointSetViewModel = new(WorldPointSetSelectionService.SelectedMapObjects, undoManagerService);
            MultiWorldPointViewModel = new(WorldPointSelectionService.SelectedMapObjects, undoManagerService);
            MultiObjectivePointViewModel = new(ObjectivePointSelectionService.SelectedMapObjects, undoManagerService);
            MultiMapTextPointViewModel = new(MapTextPointSelectionService.SelectedMapObjects, undoManagerService);
            currenSelectionKBShortcutService = worldObjectSelectionKBShortcutService;
            currentMultiMovableMapObject = MultiWorldObjectViewModel;
            currentMultiRotatableMapObject = MultiWorldObjectViewModel;
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
                (Key.D1, ModifierKeys.None, ToggleSelectActionCommand),
                (Key.NumPad1, ModifierKeys.None, ToggleSelectActionCommand),
                (Key.D2, ModifierKeys.None, ToggleMoveActionCommand),
                (Key.NumPad2, ModifierKeys.None, ToggleMoveActionCommand),
                (Key.D3, ModifierKeys.None, ToggleRotateActionCommand),
                (Key.NumPad3, ModifierKeys.None, ToggleRotateActionCommand),
                (Key.D4, ModifierKeys.None, ToggleAlignActionCommand),
                (Key.NumPad4, ModifierKeys.None, ToggleAlignActionCommand),
                (Key.D5, ModifierKeys.None, ToggleDistributeActionCommand),
                (Key.NumPad5, ModifierKeys.None, ToggleDistributeActionCommand),
            };
            undoManagerService.PropertyChanged += UndoManagerService_PropertyChanged;
            WorldObjectSelectionService.SelectionChanged += WorldObjectSelectionService_SelectionChanged;
            PlayerSelectionService.SelectionChanged += PlayerSelectionService_SelectionChanged;
            WaypointPathSelectionService.SelectionChanged += WaypointPathSelectionService_SelectionChanged;
            WaypointPathPointSelectionService.SelectionChanged += WaypointPathPointSelectionService_SelectionChanged;
            WorldPolygonSelectionService.SelectionChanged += WorldPolygonSelectionService_SelectionChanged;
            WorldPolygonPointSelectionService.SelectionChanged += WorldPolygonPointSelectionService_SelectionChanged;
            WorldPointSetSelectionService.SelectionChanged += WorldPointSetSelectionService_SelectionChanged;
            WorldPointSelectionService.SelectionChanged += WorldPointSelectionService_SelectionChanged;
            ObjectivePointSelectionService.SelectionChanged += ObjectivePointSelectionService_SelectionChanged;
            MapTextPointSelectionService.SelectionChanged += MapTextPointSelectionService_SelectionChanged;
        }

        #region MenuCommands

        [RelayCommand]
        private void OnMapOpenNew()
        {
            var ofd = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = ".twt",
                Filter = "Map file (.twt)|*.twt",
                Title = "Select a map file",
            };
            if (ofd.ShowDialog(Application.Current.MainWindow) == true)
            {
                if (MessageBox.Show("The current map will be cleared. Continue ?", "Map import", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    MapReset();
                    WorldMap? tempMap = null;
                    new ProgressDialog(Application.Current.MainWindow, "Import map").RunAction((progress, progressLogs) =>
                    {
                        using var di = new DataImport(ofd.FileName, Map, progressLogs, progress, ruleConditionCopyPasteService, ruleActionCopyPasteService, waypointPathPointCopyPasteService, worldPolygonPointCopyPasteService, worldPointCopyPasteService);
                        tempMap = di.ReadMapFileAndAddData();
                    });
                    if(tempMap != null)
                    {
                        var vm = new ImportMapViewModel(Map, true);
                        vm.ImportMap(tempMap);
                    }
                }
            }
        }

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
            if (ofd.ShowDialog(Application.Current.MainWindow) == true)
            {
                WorldMap? tempMap = null;
                new ProgressDialog(Application.Current.MainWindow, "Import map").RunAction((progress, progressLogs) =>
                {
                    using var di = new DataImport(ofd.FileName, Map, progressLogs, progress, ruleConditionCopyPasteService, ruleActionCopyPasteService, waypointPathPointCopyPasteService, worldPolygonPointCopyPasteService, worldPointCopyPasteService);
                    tempMap = di.ReadMapFileAndAddData();
                }, true, false);
                if (tempMap != null)
                {
                    var vm = new ImportMapViewModel(Map);
                    var shouldContinue = true;
                    while (shouldContinue)
                    {
                        if (new ImportMapDialog(Application.Current.MainWindow, "Map import settings") { DataContext = vm }.ShowDialog() == true)
                        {
                            vm.ImportMap(tempMap);
                            shouldContinue = false;
                        }
                        else
                        {
                            if(MessageBox.Show("Do you want to cancel the import ?", "Cancel map import ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                shouldContinue = false;
                            }
                        }
                    }
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
            if (sfd.ShowDialog(Application.Current.MainWindow) == true)
            {
                new ProgressDialog(Application.Current.MainWindow, "Export map").RunAction((progress, progressLogs) =>
                {
                    try
                    {
                        if (settings.CreateBackupOnMapExport && File.Exists(sfd.FileName))
                        {
                            // Create backup file
                            File.Copy(sfd.FileName, sfd.FileName + ".bak", true);
                        }
                        ValidateMap(progressLogs);
                        using (var de = new DataExport(sfd.FileName, Map, progressLogs, progress))
                        {
                            de.CreateMapFileAndWriteData();
                        }
                    }
                    catch (Exception ex)
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
            new StarmapExportDialog(Application.Current.MainWindow, "Starmap preview") { DataContext = new StarmapExportViewModel(Map) }.ShowDialog();
        }

        [RelayCommand]
        private void OnResetMap()
        {
            if (MessageBox.Show("The current map will be cleared. Continue ?", "Map reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MapReset();
            }
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(Application.Current.MainWindow, "Map size", Map.Size, Map.ZSize, Map.WorldBuffer);
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
            new WorldInfoDialog(Application.Current.MainWindow, "World info", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnFlagsEdit()
        {
            copyPasteService.ClearClipboard();
            new FlagDialog(Application.Current.MainWindow, "Flags") { DataContext = new CollectionEditorViewModel<Flag>(Map.Flags, () => new Flag(Map), copyPasteService, true) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnGroupsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Groups") { DataContext = new CollectionEditorViewModel<Group>(Map.Groups, () => new Group(Map), copyPasteService) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnJournalEntriesEdit()
        {
            copyPasteService.ClearClipboard();
            new JournalEntryDialog(Application.Current.MainWindow, "Journal entries") { DataContext = new CollectionEditorViewModel<JournalEntry>(Map.JournalEntries, () => new JournalEntry(Map, StringDictionnary.SpeechEventsJournals.Keys.FirstOrDefault(), AppSettings.DialogueFilesList.FirstOrDefault(), AppSettings.HudTexturesList.FirstOrDefault()), copyPasteService, true) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnMapTextPointsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Map text points") { DataContext = new CollectionEditorViewModel<MapTextPoint>(Map.MapTextPoints, () => new MapTextPoint(Map, NamedObject.GenerateName("MapTextPoint", Map.MapTextPoints), StringDictionnary.MapTextItems.Keys.FirstOrDefault()), copyPasteService) }.ShowDialog();
            if (MapTextPointSelectionService.SelectedMapObject != null)
            {
                if (!Map.MapTextPoints.Contains(MapTextPointSelectionService.SelectedMapObject))
                {
                    MapTextPointSelectionService.RemoveFromSelection(MapTextPointSelectionService.SelectedMapObject);
                }
            }
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnObjectiveTasksEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Objective tasks") { DataContext = new CollectionEditorViewModel<ObjectiveTask>(Map.ObjectiveTasks, () => new ObjectiveTask(Map, NamedObject.GenerateName("ObjectiveTask", Map.ObjectiveTasks), StringDictionnary.ObjectiveTasks.Keys.FirstOrDefault()), copyPasteService) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnObjectivePointsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Objective points") { DataContext = new CollectionEditorViewModel<ObjectivePoint>(Map.ObjectivePoints, () => new ObjectivePoint(Map), copyPasteService) }.ShowDialog();
            if (ObjectivePointSelectionService.SelectedMapObject != null)
            {
                if (!Map.ObjectivePoints.Contains(ObjectivePointSelectionService.SelectedMapObject))
                {
                    ObjectivePointSelectionService.RemoveFromSelection(ObjectivePointSelectionService.SelectedMapObject);
                }
            }
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Players") { DataContext = new CollectionEditorViewModel<Player>(Map.Players, () => new Player(Map), copyPasteService) }.ShowDialog();
            if (PlayerSelectionService.SelectedMapObject != null)
            {
                if (!Map.Players.Contains(PlayerSelectionService.SelectedMapObject))
                {
                    PlayerSelectionService.RemoveFromSelection(PlayerSelectionService.SelectedMapObject);
                }
            }
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            copyPasteService.ClearClipboard();
            new PlayerAllianceDialog(Application.Current.MainWindow, "Player alliances") { DataContext = new CollectionEditorViewModel<PlayerAlliance>(Map.PlayerAlliances, () => new PlayerAlliance(Map, Map.SelectablePlayers, Player.DefaultPlayer, Player.DefaultPlayer), copyPasteService, true) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnSpeechEventsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Speech events") { DataContext = new CollectionEditorViewModel<SpeechEvent>(Map.SpeechEvents, () => new SpeechEvent(Map, NamedObject.GenerateName("SpeechEvent", Map.SpeechEvents)), copyPasteService) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnTeamsEdit()
        {
            copyPasteService.ClearClipboard();
            new TeamsDialog(Application.Current.MainWindow, "Teams") { DataContext = new TeamEditorViewModel(Map.SelectableTeams, Map.InGameTeams, () => new Team(Map, StringDictionnary.TeamNames.Keys.FirstOrDefault()), copyPasteService) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnTimersEdit()
        {
            copyPasteService.ClearClipboard();
            new TimerDialog(Application.Current.MainWindow, "Timers") { DataContext = new CollectionEditorViewModel<Timer>(Map.Timers, () => new Timer(Map), copyPasteService, true) }.ShowDialog();
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnWaypointPathsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "Waypoint paths")
            {
                DataContext = new CollectionEditorViewModel<WaypointPath>(Map.WaypointPaths, () =>
                {
                    var wp = new WaypointPath(Map, NamedObject.GenerateName("WaypointPath", Map.WaypointPaths), waypointPathPointCopyPasteService);
                    wp.Points.Add(new(wp, 0, 0, 0));
                    return wp;
                }, copyPasteService)
            }.ShowDialog();
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
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnWorldCrewsAndArmsEdit()
        {
            new WorldCrewAndArmsDialog(Application.Current.MainWindow, "World crews and arms", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "World objects") { DataContext = new CollectionEditorViewModel<WorldObject>(Map.WorldObjects, () => new WorldObject(Map), copyPasteService) }.ShowDialog();
            if (WorldObjectSelectionService.SelectedMapObject != null)
            {
                if (!Map.WorldObjects.Contains(WorldObjectSelectionService.SelectedMapObject))
                {
                    WorldObjectSelectionService.RemoveFromSelection(WorldObjectSelectionService.SelectedMapObject);
                }
            }
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnWorldPointSetsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "World point sets")
            {
                DataContext = new CollectionEditorViewModel<WorldPointSet>(Map.WorldPointSets, () =>
                {
                    var wps = new WorldPointSet(Map, NamedObject.GenerateName("WorldPointSet", Map.WorldPointSets), worldPointCopyPasteService);
                    wps.Points.Add(new(wps, 0, 0, 0, 0));
                    return wps;
                }, copyPasteService)
            }.ShowDialog();
            if (WorldPointSetSelectionService.SelectedMapObject != null)
            {
                if (!Map.WorldPointSets.Contains(WorldPointSetSelectionService.SelectedMapObject))
                {
                    WorldPointSetSelectionService.RemoveFromSelection(WorldPointSetSelectionService.SelectedMapObject);
                    if (WorldPointSelectionService.SelectedMapObject != null)
                        WorldPointSelectionService.RemoveFromSelection(WorldPointSelectionService.SelectedMapObject);
                }
                else if (WorldPointSelectionService.SelectedMapObject != null && !WorldPointSetSelectionService.SelectedMapObject.Points.Contains(WorldPointSelectionService.SelectedMapObject))
                {
                    WorldPointSelectionService.RemoveFromSelection(WorldPointSelectionService.SelectedMapObject);
                }
            }
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnWorldPolygonsEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "World polygons")
            {
                DataContext = new CollectionEditorViewModel<WorldPolygon>(Map.WorldPolygons, () =>
                {
                    var wp = new WorldPolygon(Map, NamedObject.GenerateName("WorldPolygon", Map.WorldPolygons), worldPolygonPointCopyPasteService);
                    wp.Points.Add(new(wp, 0, 0));
                    return wp;
                }, copyPasteService)
            }.ShowDialog();
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
            copyPasteService.ClearClipboard();
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            copyPasteService.ClearClipboard();
            new CollectionEditorDialog(Application.Current.MainWindow, "World rules") { DataContext = new CollectionEditorViewModel<WorldRule>(Map.WorldRules, () => new WorldRule(Map, NamedObject.GenerateName("WorldRule", Map.WorldRules), ruleConditionCopyPasteService, ruleActionCopyPasteService), copyPasteService) }.ShowDialog();
            copyPasteService.ClearClipboard();
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
            var asd = new AppSettingsDialog(Application.Current.MainWindow, "Settings", settings);
            asd.ShowDialog();
            settings.Save();
            if (settings.TpGamePath != tpGamePath)
            {
                new ProgressDialog(Application.Current.MainWindow, "Reload TPGame folder").RunActionSameThread((progress, progressLogs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadDialogueFilesList();
                    progress.Report("Reloading complete");
                }
                catch (Exception ex)
                {
                    logs.Report($"Error: {ex.Message}");
                }
            }, true);
        }

        [RelayCommand]
        private void OnReloadEffectList()
        {
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
            {
                try
                {
                    progress.Report("Reloading ...");
                    settings.ReloadStringsDictionnaries(progress, logs);
                    progress.Report("Reloading complete");
                    SelectedMapTextPointPreviewTextIndex = 0;
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
            new ProgressDialog(Application.Current.MainWindow, "Reload").RunActionSameThread((progress, logs) =>
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

        [RelayCommand]
        private void OnToggleSelectAction()
        {
            IsSelectCommandActive = !IsSelectCommandActive;
        }

        [RelayCommand]
        private void OnToggleMoveAction()
        {
            IsMoveCommandActive = !IsMoveCommandActive;
        }

        [RelayCommand]
        private void OnToggleRotateAction()
        {
            IsRotateCommandActive = !IsRotateCommandActive;
        }

        [RelayCommand]
        private void OnToggleAlignAction()
        {
            IsAlignCommandActive = !IsAlignCommandActive;
        }

        [RelayCommand]
        private void OnToggleDistributeAction()
        {
            IsDistributeCommandActive = !IsDistributeCommandActive;
        }

        #endregion

        #region UtilMethods

        private void MapReset()
        {
            Map.Reset();
            ClearSelections();
            undoManagerService.Clear();
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
        /// Execute the keyboard shortcut if it exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns>True if executed, false otherwise</returns>
        private ICommand? GetKBShortcutCommand(Key key, ModifierKeys modifiers)
        {
            foreach (var kbShortcut in keyboardShortcut)
            {
                if (kbShortcut.key == key && kbShortcut.modifiers == modifiers)
                {
                    return kbShortcut.command;
                }
            }
            return null;
        }

        public void LoadSettings()
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
        }

        public void SaveSettings()
        {
            settings.Save();
        }

        /// <summary>
        /// Reload all the app settings
        /// </summary>
        /// <param name="title"></param>
        /// <param name="notifyOnFinish"></param>
        private void ReloadAllSettings(string title, bool notifyOnFinish = true)
        {
            new ProgressDialog(Application.Current.MainWindow, title).RunActionSameThread((progress, progressLogs) =>
            {
                progress.Report("Reloading ...");
                settings.ReloadAll(progress, progressLogs);
                progress.Report("Reloading complete");
                SelectedMapTextPointPreviewTextIndex = 0;
            }, true, notifyOnFinish);
        }

        public bool TryExecuteKBShortcutCommand(Key key, ModifierKeys modifiers)
        {
            var command = GetKBShortcutCommand(key, modifiers);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
                return true;
            }
            return false;
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

        public void InitAlignTransformCommand(bool alignOnX = false, bool alignOnY = false, bool alignOnZ = false, double x = 0, double y = 0, double z = 0)
        {
            ClearAlignTransformMapCommand();
            if (currentMultiMovableMapObject.Count > 0)
            {
                alignTransformMapCommand = new(currentMultiMovableMapObject, IsMovableSelection3D) { AlignOnX = alignOnX, AlignOnY = alignOnY, AlignOnZ = alignOnZ, X = x, Y = y, Z = z };
                alignTransformMapCommand.PropertyChanged += AlignTransformMapCommand_PropertyChanged;
                CurrentMapCommand = alignTransformMapCommand;
                shouldCommitTransformMapCommand = hasCommittedTransformMapCommand = false;
                TransformMapCommandTitle = "Align";
            }
        }

        private void ClearAlignTransformMapCommand()
        {
            if (alignTransformMapCommand != null)
                alignTransformMapCommand.PropertyChanged -= AlignTransformMapCommand_PropertyChanged;
            CurrentMapCommand = alignTransformMapCommand = null;
        }

        private void AlignTransformMapCommand_PropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (!alignTransformMapCommand!.CanUndo)
                InitAlignTransformCommand(alignTransformMapCommand.AlignOnX, alignTransformMapCommand.AlignOnY, alignTransformMapCommand.AlignOnZ, alignTransformMapCommand.X, alignTransformMapCommand.Y, alignTransformMapCommand.Z);
            shouldCommitTransformMapCommand = true;
            if (shouldCommitTransformMapCommand && !hasCommittedTransformMapCommand)
            {
                undoManagerService.Push(alignTransformMapCommand!);
                hasCommittedTransformMapCommand = true;
            }
            alignTransformMapCommand!.Apply();
        }

        public void AlignTransformSelection(double x, double y)
        {
            if (alignTransformMapCommand != null)
            {
                alignTransformMapCommand!.X += x;
                alignTransformMapCommand!.Y += y;
            }
        }

        public void InitDistributeTransformCommand(bool distributeOnX = false, bool distributeOnY = false, bool distributeOnZ = false, double x = 0, double y = 0, double z = 0)
        {
            ClearDistributeTransformMapCommand();
            if (currentMultiMovableMapObject.Count > 0)
            {
                distributeTransformMapCommand = new(currentMultiMovableMapObject, IsMovableSelection3D) { DistributeOnX = distributeOnX, DistributeOnY = distributeOnY, DistributeOnZ = distributeOnZ, X = x, Y = y, Z = z };
                distributeTransformMapCommand.PropertyChanged += DistributeTransformMapCommand_PropertyChanged;
                CurrentMapCommand = distributeTransformMapCommand;
                shouldCommitTransformMapCommand = hasCommittedTransformMapCommand = false;
                TransformMapCommandTitle = "Distribute";
            }
        }

        private void ClearDistributeTransformMapCommand()
        {
            if (distributeTransformMapCommand != null)
                distributeTransformMapCommand.PropertyChanged -= DistributeTransformMapCommand_PropertyChanged;
            CurrentMapCommand = distributeTransformMapCommand = null;
        }

        private void DistributeTransformMapCommand_PropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (!distributeTransformMapCommand!.CanUndo)
                InitDistributeTransformCommand(distributeTransformMapCommand.DistributeOnX, distributeTransformMapCommand.DistributeOnY, distributeTransformMapCommand.DistributeOnZ, distributeTransformMapCommand.X, distributeTransformMapCommand.Y, distributeTransformMapCommand.Z);
            shouldCommitTransformMapCommand = true;
            if (shouldCommitTransformMapCommand && !hasCommittedTransformMapCommand)
            {
                undoManagerService.Push(distributeTransformMapCommand!);
                hasCommittedTransformMapCommand = true;
            }
            distributeTransformMapCommand!.Apply();
        }

        public void DistributeTransformSelection(double x, double y)
        {
            if (distributeTransformMapCommand != null)
            {
                distributeTransformMapCommand!.X += x;
                distributeTransformMapCommand!.Y += y;
            }
        }

        public void InitTranslateTransformCommand(double x = 0, double y = 0, double z = 0)
        {
            ClearTranslateTransformMapCommand();
            if (currentMultiMovableMapObject.Count > 0)
            {
                translateTransformMapCommand = new(currentMultiMovableMapObject, IsMovableSelection3D) { DeltaX = x, DeltaY = y, DeltaZ = z };
                translateTransformMapCommand.PropertyChanged += TranslateTransformMapCommand_PropertyChanged;
                CurrentMapCommand = translateTransformMapCommand;
                shouldCommitTransformMapCommand = hasCommittedTransformMapCommand = false;
                TransformMapCommandTitle = "Move";
            }
        }
        
        private void ClearTranslateTransformMapCommand()
        {
            if (translateTransformMapCommand != null)
                translateTransformMapCommand.PropertyChanged -= TranslateTransformMapCommand_PropertyChanged;
            CurrentMapCommand = translateTransformMapCommand = null;
        }

        private void TranslateTransformMapCommand_PropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (!translateTransformMapCommand!.CanUndo)
                InitTranslateTransformCommand(translateTransformMapCommand.DeltaX, translateTransformMapCommand.DeltaY, translateTransformMapCommand.DeltaZ);
            shouldCommitTransformMapCommand = true;
            if (shouldCommitTransformMapCommand && !hasCommittedTransformMapCommand)
            {
                undoManagerService.Push(translateTransformMapCommand!);
                hasCommittedTransformMapCommand = true;
            }
            translateTransformMapCommand!.Apply();
        }

        public void TranslateTransformSelection(double x, double y)
        {
            if (translateTransformMapCommand != null)
            {
                translateTransformMapCommand!.DeltaX += x;
                translateTransformMapCommand!.DeltaY += y;
            }
        }

        public void InitRotateTransformMapCommand()
        {
            //Rotation on IRotatableMapObject
            if (CanChangeRotationMode)
            {
                InitRotateOrbitSpinTransformMapCommand();
            }
            //Rotation on IMovableMapObject
            else
            {
                InitRotateOrbitTransformMapCommand();
            }
        }

        private void InitRotateOrbitSpinTransformMapCommand(double rotation = 0, bool isRotationOrbit = true, bool isRotationSpin = true)
        {
            ClearRotateOrbitSpinTransformMapCommand();
            if (currentMultiRotatableMapObject != null && currentMultiRotatableMapObject.Count > 0)
            {
                rotateOrbitSpinTransformMapCommand = new(currentMultiRotatableMapObject) { Rotation = rotation, IsRotationOrbit = isRotationOrbit, IsRotationSpin = isRotationSpin };
                rotateOrbitSpinTransformMapCommand.PropertyChanged += RotateOrbitSpinTransformMapCommand_PropertyChanged;
                CurrentMapCommand = rotateOrbitSpinTransformMapCommand;
                shouldCommitTransformMapCommand = hasCommittedTransformMapCommand = false;
                TransformMapCommandTitle = "Rotate";
            }
        }

        private void RotateOrbitSpinTransformMapCommand_PropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (!rotateOrbitSpinTransformMapCommand!.CanUndo)
                InitRotateOrbitSpinTransformMapCommand(rotateOrbitSpinTransformMapCommand.Rotation, rotateOrbitSpinTransformMapCommand.IsRotationOrbit, rotateOrbitSpinTransformMapCommand.IsRotationSpin);
            shouldCommitTransformMapCommand = true;
            if (shouldCommitTransformMapCommand && !hasCommittedTransformMapCommand)
            {
                undoManagerService.Push(rotateOrbitSpinTransformMapCommand!);
                hasCommittedTransformMapCommand = true;
            }
            rotateOrbitSpinTransformMapCommand!.Apply();
        }

        private void ClearRotateOrbitSpinTransformMapCommand()
        {
            if (rotateOrbitSpinTransformMapCommand != null)
                rotateOrbitSpinTransformMapCommand.PropertyChanged -= RotateOrbitSpinTransformMapCommand_PropertyChanged;
            CurrentMapCommand = rotateOrbitSpinTransformMapCommand = null;
        }

        private void InitRotateOrbitTransformMapCommand(double rotation = 0)
        {
            ClearRotateOrbitTransformMapCommand();
            if (currentMultiMovableMapObject.Count > 0)
            {
                rotateOrbitTransformMapCommand = new(currentMultiMovableMapObject) { Rotation = rotation };
                rotateOrbitTransformMapCommand.PropertyChanged += RotateOrbitTransformMapCommand_PropertyChanged;
                CurrentMapCommand = rotateOrbitTransformMapCommand;
                shouldCommitTransformMapCommand = hasCommittedTransformMapCommand = false;
                TransformMapCommandTitle = "Rotate";
            }
        }

        private void RotateOrbitTransformMapCommand_PropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (!rotateOrbitTransformMapCommand!.CanUndo)
                InitRotateOrbitTransformMapCommand(rotateOrbitTransformMapCommand.Rotation);
            shouldCommitTransformMapCommand = true;
            if (shouldCommitTransformMapCommand && !hasCommittedTransformMapCommand)
            {
                undoManagerService.Push(rotateOrbitTransformMapCommand!);
                hasCommittedTransformMapCommand = true;
            }
            rotateOrbitTransformMapCommand!.Apply();
        }

        private void ClearRotateOrbitTransformMapCommand()
        {
            if (rotateOrbitTransformMapCommand != null)
                rotateOrbitTransformMapCommand.PropertyChanged -= RotateOrbitTransformMapCommand_PropertyChanged;
            CurrentMapCommand = rotateOrbitTransformMapCommand = null;
        }

        public void RotateTransformSelection(double rotation)
        {
            if (CanChangeRotationMode)
                if (rotateOrbitSpinTransformMapCommand != null)
                    rotateOrbitSpinTransformMapCommand.Rotation = GetRotation(rotateOrbitSpinTransformMapCommand.Rotation + rotation);
                else
                    InitRotateOrbitSpinTransformMapCommand(GetRotation(rotation));
            else
                if (rotateOrbitTransformMapCommand != null)
                    rotateOrbitTransformMapCommand.Rotation = GetRotation(rotateOrbitTransformMapCommand.Rotation + rotation);
                else
                    InitRotateOrbitTransformMapCommand(GetRotation(rotation));
        }

        private void InitTransformCommandsIfPossible()
        {
            if (IsMoveCommandActive)
            {
                InitTranslateTransformCommand();
            }
            else if (IsRotateCommandActive)
            {
                InitRotateTransformMapCommand();
            }
            else if (IsAlignCommandActive)
            {
                InitAlignTransformCommand();
            }
            else if (IsDistributeCommandActive)
            {
                InitDistributeTransformCommand();
            }
        }

        private void ClearTransformMapCommands()
        {
            ClearTranslateTransformMapCommand();
            ClearRotateOrbitSpinTransformMapCommand();
            ClearRotateOrbitTransformMapCommand();
            ClearAlignTransformMapCommand();
            ClearDistributeTransformMapCommand();
        }

        private void WorldObjectSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWorldObjectViewModel.UpdateFromMapObject(WorldObjectSelectionService.SelectedMapObject);
        }

        private void PlayerSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiPlayerViewModel.UpdateFromMapObject(PlayerSelectionService.SelectedMapObject);
        }

        private void WaypointPathSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWaypointPathViewModel.UpdateFromMapObject(WaypointPathSelectionService.SelectedMapObject);
        }

        private void WaypointPathPointSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWaypointPathPointViewModel.UpdateFromMapObject(WaypointPathPointSelectionService.SelectedMapObject);
        }

        private void WorldPolygonSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWorldPolygonViewModel.UpdateFromMapObject(WorldPolygonSelectionService.SelectedMapObject);
        }

        private void WorldPolygonPointSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWorldPolygonPointViewModel.UpdateFromMapObject(WorldPolygonPointSelectionService.SelectedMapObject);
        }

        private void WorldPointSetSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWorldPointSetViewModel.UpdateFromMapObject(WorldPointSetSelectionService.SelectedMapObject);
        }

        private void WorldPointSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiWorldPointViewModel.UpdateFromMapObject(WorldPointSelectionService.SelectedMapObject);
        }

        private void ObjectivePointSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiObjectivePointViewModel.UpdateFromMapObject(ObjectivePointSelectionService.SelectedMapObject);
        }

        private void MapTextPointSelectionService_SelectionChanged(object s, EventArgs e)
        {
            InitTransformCommandsIfPossible();
            MultiMapTextPointViewModel.UpdateFromMapObject(MapTextPointSelectionService.SelectedMapObject);
        }

        partial void OnIsMoveCommandActiveChanging(bool value)
        {
            if (value)
            {
                IsRotateCommandActive = IsAlignCommandActive = IsDistributeCommandActive = false;
                InitTranslateTransformCommand();
            }
            else
            {
                ClearTranslateTransformMapCommand();
            }
        }

        partial void OnIsRotateCommandActiveChanging(bool value)
        {
            if (value)
            {
                IsMoveCommandActive = IsAlignCommandActive = IsDistributeCommandActive = false;
                InitRotateTransformMapCommand();
            }
            else
            {
                ClearRotateOrbitTransformMapCommand();
                ClearRotateOrbitSpinTransformMapCommand();
            }
        }

        partial void OnIsAlignCommandActiveChanging(bool value)
        {
            if (value)
            {
                IsRotateCommandActive = IsMoveCommandActive = IsDistributeCommandActive = false;
                InitAlignTransformCommand();
            }
            else
            {
                ClearAlignTransformMapCommand();
            }
        }

        partial void OnIsDistributeCommandActiveChanging(bool value)
        {
            if (value)
            {
                IsRotateCommandActive = IsAlignCommandActive = IsMoveCommandActive = false;
                InitDistributeTransformCommand();
            }
            else
            {
                ClearDistributeTransformMapCommand();
            }
        }

        #endregion

        #region WorldObject

        public void ActivateWorldObjects()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = worldObjectSelectionKBShortcutService;
            currentMultiMovableMapObject = currentMultiRotatableMapObject = MultiWorldObjectViewModel;
            IsMovableSelection3D = true;
            CanChangeRotationMode = true;
            ClearTransformMapCommands();
        }

        public void ClearWorldObjectSelection()
        {
            WorldObjectSelectionService.ClearSelection();
        }

        public void SelectWorldObjectsInRect(Rect rect)
        {
            for (int i = 0; i < Map.WorldObjects.Count; i++)
            {
                var obj = Map.WorldObjects[i];
                if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                    WorldObjectSelectionService.SelectAndMakeLastSelected(obj);
            }
        }

        public void SelectWorldObject(object obj, bool ctrlPressed)
        {
            if(obj is WorldObject worldObject)
            {
                if (ctrlPressed)
                {
                    WorldObjectSelectionService.CtrlSelect(worldObject);
                }
                else
                {
                    WorldObjectSelectionService.ClearSelection();
                    WorldObjectSelectionService.SelectAndMakeLastSelected(worldObject);
                }
            }
        }

        public void RemoveSelectedWorldObjectsFromMap()
        {
            var selectedItems = WorldObjectSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.WorldObjects.Remove(item);
            }
            WorldObjectSelectionService.ClearSelection();
        }

        public void CreateWorldObject(double x, double y, double z, double zRotation)
        {
            var zRotationRad = zRotation * Math.PI / 180;
            x -= Math.Cos(zRotationRad) * SelectedWorldObjectType!.Pivot.X + Math.Sin(zRotationRad) * SelectedWorldObjectType!.Pivot.Y;
            y += Math.Cos(zRotationRad) * SelectedWorldObjectType!.Pivot.Y - Math.Sin(zRotationRad) * SelectedWorldObjectType!.Pivot.X;
            var wot = new WorldObject(Map, SelectedWorldObjectType!, x, y, z, zRotation);
            Map.WorldObjects.Add(wot);
            WorldObjectSelectionService.SelectAndMakeLastSelected(wot);
        }

        public void SetAllWorldObjectsVisibility(bool isVisible)
        {
            foreach (var obj in Map.WorldObjects)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region Player

        public void ActivatePlayers()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = playerSelectionKBShortcutService;
            currentMultiMovableMapObject = currentMultiRotatableMapObject = MultiPlayerViewModel;
            IsMovableSelection3D = true;
            CanChangeRotationMode = true;
            ClearTransformMapCommands();
        }

        public void ClearPlayerSelection()
        {
            PlayerSelectionService.ClearSelection();
        }

        public void SelectPlayersInRect(Rect rect)
        {
            for (int i = 0; i < Map.Players.Count; i++)
            {
                var obj = Map.Players[i];
                if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                    PlayerSelectionService.SelectAndMakeLastSelected(obj);
            }
        }

        public void SelectPlayer(object obj, bool ctrlPressed)
        {
            if (obj is Player player)
            {
                if (ctrlPressed)
                {
                    PlayerSelectionService.CtrlSelect(player);
                }
                else
                {
                    PlayerSelectionService.ClearSelection();
                    PlayerSelectionService.SelectAndMakeLastSelected(player);
                }
            }
        }

        public void RemoveSelectedPlayersFromMap()
        {
            var selectedItems = PlayerSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.Players.Remove(item);
            }
            PlayerSelectionService.ClearSelection();
        }

        public void CreatePlayer(double x, double y, double z, double rotation)
        {
            var player = new Player(Map, NamedObject.GenerateName("Player", Map.Players), x, y, z, rotation, Colors.Red);
            Map.Players.Add(player);
            PlayerSelectionService.SelectAndMakeLastSelected(player);
        }

        public void SetAllPlayersVisibility(bool isVisible)
        {
            foreach (var obj in Map.Players)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region WaypointPath

        public void ActivateWaypointPaths()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = waypointPathSelectionKBShortcutService;
            currentMultiMovableMapObject = MultiWaypointPathPointViewModel;
            currentMultiRotatableMapObject = null;
            IsMovableSelection3D = true;
            CanChangeRotationMode = false;
            ClearTransformMapCommands();
        }

        public void ClearWaypointPathSelection()
        {
            WaypointPathSelectionService.ClearSelection();
            WaypointPathPointSelectionService.ClearSelection();
        }

        public void SelectWaypointPathPointsInRect(Rect rect)
        {
            for (int i = 0; i < Map.WaypointPaths.Count; i++)
            {
                var path = Map.WaypointPaths[i];
                if (path.IsShownOnUi)
                {
                    for (int j = 0; j < path.Points.Count; j++)
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
        }

        public void SelectWaypointPathOrWaypointPathPoint(object obj, bool ctrlPressed)
        {
            if(obj is WaypointPath path)
            {
                SelectWaypointPath_Internal(path, ctrlPressed);
            }
            else if(obj is WaypointPathPoint point)
            {
                SelectWaypointPathPoint_Internal(point, ctrlPressed);
            }
        }

        public void SelectWaypointPath(object obj, bool ctrlPressed)
        {
            if(obj is WaypointPath path)
            {
                SelectWaypointPath_Internal(path, ctrlPressed);
            }
        }

        public void SelectWaypointPathPoint(object obj, bool ctrlPressed)
        {
            if(obj is WaypointPathPoint point)
            {
                SelectWaypointPathPoint_Internal(point, ctrlPressed);
            }
        }

        private void SelectWaypointPath_Internal(WaypointPath path, bool ctrlPressed)
        {
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

        private void SelectWaypointPathPoint_Internal(WaypointPathPoint point, bool ctrlPressed)
        {
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

        public void RemoveSelectedWaypointPathPointsFromMap()
        {
            foreach (var p in WaypointPathPointSelectionService.SelectedMapObjects)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WaypointPaths.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        WaypointPathSelectionService.RemoveFromSelection(p.Parent);
                    }
                }
            }
            ClearWaypointPathSelection();
        }

        public void CreateWaypointPath(double x, double y, double z = 0)
        {
            var waypointPath = new WaypointPath(Map, NamedObject.GenerateName("WaypointPath", Map.WaypointPaths), copyPasteService);
            var point = new WaypointPathPoint(waypointPath, x, y, z);
            waypointPath.Points.Add(point);
            Map.WaypointPaths.Add(waypointPath);
            WaypointPathSelectionService.ClearSelection();
            WaypointPathPointSelectionService.ClearSelection();
            WaypointPathSelectionService.SelectAndMakeLastSelected(waypointPath);
            WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
        }

        public void AddWaypointPathPointToSelectedWaypointPath(double x, double y, double z = 0)
        {
            if (WaypointPathSelectionService.SelectedMapObject != null)
            {
                var point = new WaypointPathPoint(WaypointPathSelectionService.SelectedMapObject, x, y, z);
                WaypointPathPointSelectionService.SelectAndMakeLastSelected(point);
                WaypointPathSelectionService.SelectedMapObject.Points.Add(point);
            }
        }

        public void SetAllWaypointPathsVisibility(bool isVisible)
        {
            foreach (var obj in Map.WaypointPaths)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region WorldPolygon

        public void ActivateWorldPolygons()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = worldPolygonSelectionKBShortcutService;
            currentMultiMovableMapObject = MultiWorldPolygonPointViewModel;
            currentMultiRotatableMapObject = null;
            IsMovableSelection3D = false;
            CanChangeRotationMode = false;
            ClearTransformMapCommands();
        }

        public void ClearWorldPolygonSelection()
        {
            WorldPolygonSelectionService.ClearSelection();
            WorldPolygonPointSelectionService.ClearSelection();
        }

        public void SelectWorldPolygonPointsInRect(Rect rect)
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
        }

        public void SelectWorldPolygonOrWorldPolygonPoint(object obj, bool ctrlPressed)
        {
            if (obj is WorldPolygon path)
            {
                SelectWorldPolygon_Internal(path, ctrlPressed);
            }
            else if (obj is WorldPolygonPoint point)
            {
                SelectWorldPolygonPoint_Internal(point, ctrlPressed);
            }
        }

        public void SelectWorldPolygon(object obj, bool ctrlPressed)
        {
            if (obj is WorldPolygon path)
            {
                SelectWorldPolygon_Internal(path, ctrlPressed);
            }
        }

        public void SelectWorldPolygonPoint(object obj, bool ctrlPressed)
        {
            if (obj is WorldPolygonPoint point)
            {
                SelectWorldPolygonPoint_Internal(point, ctrlPressed);
            }
        }

        private void SelectWorldPolygon_Internal(WorldPolygon path, bool ctrlPressed)
        {
            if (ctrlPressed)
            {
                if (path.IsLastSelected)
                {
                    WorldPolygonSelectionService.RemoveFromSelection(path);
                }
                else
                {
                    WorldPolygonSelectionService.SelectAndMakeLastSelected(path);
                }
            }
            else
            {
                WorldPolygonSelectionService.ClearSelection();
                WorldPolygonPointSelectionService.ClearSelection();
                WorldPolygonSelectionService.SelectAndMakeLastSelected(path);
                foreach (var item in path.Points)
                {
                    WorldPolygonPointSelectionService.SelectAndMakeLastSelected(item);
                }
            }
        }

        private void SelectWorldPolygonPoint_Internal(WorldPolygonPoint point, bool ctrlPressed)
        {
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

        public void RemoveSelectedWorldPolygonPointsFromMap()
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
            ClearWorldPolygonSelection();
        }

        public void CreateWorldPolygon(double x, double y)
        {
            var worldPolygon = new WorldPolygon(Map, NamedObject.GenerateName("WorldPolygon", Map.WorldPolygons), copyPasteService);
            var point = new WorldPolygonPoint(worldPolygon, x, y);
            worldPolygon.Points.Add(point);
            Map.WorldPolygons.Add(worldPolygon);
            WorldPolygonSelectionService.ClearSelection();
            WorldPolygonPointSelectionService.ClearSelection();
            WorldPolygonSelectionService.SelectAndMakeLastSelected(worldPolygon);
            WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
        }

        public void AddWorldPolygonPointToSelectedWorldPolygon(double x, double y)
        {
            if (WorldPolygonSelectionService.SelectedMapObject != null)
            {
                var point = new WorldPolygonPoint(WorldPolygonSelectionService.SelectedMapObject, x, y);
                WorldPolygonPointSelectionService.SelectAndMakeLastSelected(point);
                WorldPolygonSelectionService.SelectedMapObject.Points.Add(point);
            }
        }

        public void SetAllWorldPolygonsVisibility(bool isVisible)
        {
            foreach (var obj in Map.WorldPolygons)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region WorldPointSet

        public void ActivateWorldPointSets()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = worldPointSetSelectionKBShortcutService;
            currentMultiMovableMapObject = currentMultiRotatableMapObject = MultiWorldPointViewModel;
            IsMovableSelection3D = true;
            CanChangeRotationMode = true;
            ClearTransformMapCommands();
        }

        public void ClearWorldPointSetSelection()
        {
            WorldPointSetSelectionService.ClearSelection();
            WorldPointSelectionService.ClearSelection();
        }

        public void SelectWorldPointsInRect(Rect rect)
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
        }

        public void SelectWorldPointSetOrWorldPoint(object obj, bool ctrlPressed)
        {
            if (obj is WorldPointSet path)
            {
                SelectWorldPointSet_Internal(path, ctrlPressed);
            }
            else if (obj is WorldPoint point)
            {
                SelectWorldPoint_Internal(point, ctrlPressed);
            }
        }

        public void SelectWorldPointSet(object obj, bool ctrlPressed)
        {
            if (obj is WorldPointSet path)
            {
                SelectWorldPointSet_Internal(path, ctrlPressed);
            }
        }

        public void SelectWorldPoint(object obj, bool ctrlPressed)
        {
            if (obj is WorldPoint point)
            {
                SelectWorldPoint_Internal(point, ctrlPressed);
            }
        }

        private void SelectWorldPointSet_Internal(WorldPointSet path, bool ctrlPressed)
        {
            if (ctrlPressed)
            {
                if (path.IsLastSelected)
                {
                    WorldPointSetSelectionService.RemoveFromSelection(path);
                }
                else
                {
                    WorldPointSetSelectionService.SelectAndMakeLastSelected(path);
                }
            }
            else
            {
                WorldPointSetSelectionService.ClearSelection();
                WorldPointSelectionService.ClearSelection();
                WorldPointSetSelectionService.SelectAndMakeLastSelected(path);
                foreach (var item in path.Points)
                {
                    WorldPointSelectionService.SelectAndMakeLastSelected(item);
                }
            }
        }

        private void SelectWorldPoint_Internal(WorldPoint point, bool ctrlPressed)
        {
            if (ctrlPressed)
            {
                if (point.IsLastSelected)
                {
                    WorldPointSelectionService.RemoveFromSelection(point);
                    WorldPointSetSelectionService.RemoveFromSelectionWithoutPoints(point.Parent);
                    if (WorldPointSelectionService.SelectedMapObject != null)
                        WorldPointSetSelectionService.MakeLastSelected(WorldPointSelectionService.SelectedMapObject.Parent);
                }
                else
                {
                    WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                    WorldPointSelectionService.SelectAndMakeLastSelected(point);
                }
            }
            else
            {
                WorldPointSetSelectionService.ClearSelection();
                WorldPointSelectionService.ClearSelection();
                WorldPointSetSelectionService.SelectAndMakeLastSelectedWithoutPoints(point.Parent);
                WorldPointSelectionService.SelectAndMakeLastSelected(point);
            }
        }

        public void RemoveSelectedWorldPointsFromMap()
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
            ClearWorldPointSetSelection();
        }

        public void CreateWorldPointSet(double x, double y, double z, double zRotation)
        {
            var worldPointSet = new WorldPointSet(Map, NamedObject.GenerateName("WorldPointSet", Map.WorldPointSets), copyPasteService);
            var point = new WorldPoint(worldPointSet, x, y, z, zRotation);
            worldPointSet.Points.Add(point);
            Map.WorldPointSets.Add(worldPointSet);
            WorldPointSetSelectionService.ClearSelection();
            WorldPointSelectionService.ClearSelection();
            WorldPointSetSelectionService.SelectAndMakeLastSelected(worldPointSet);
            WorldPointSelectionService.SelectAndMakeLastSelected(point);
        }

        public void AddWorldPointToSelectedWorldPointSet(double x, double y, double z, double zRotation)
        {
            if (WorldPointSetSelectionService.SelectedMapObject != null)
            {
                var point = new WorldPoint(WorldPointSetSelectionService.SelectedMapObject, x, y, z, zRotation);
                WorldPointSelectionService.SelectAndMakeLastSelected(point);
                WorldPointSetSelectionService.SelectedMapObject.Points.Add(point);
            }
        }

        public void SetAllWorldPointSetsVisibility(bool isVisible)
        {
            foreach (var obj in Map.WorldPointSets)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region ObjectivePoint

        public void ActivateObjectivePoints()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = objectivePointSelectionKBShortcutService;
            currentMultiMovableMapObject = MultiObjectivePointViewModel;
            currentMultiRotatableMapObject = null;
            IsMovableSelection3D = true;
            CanChangeRotationMode = false;
            ClearTransformMapCommands();
        }

        public void ClearObjectivePointSelection()
        {
            ObjectivePointSelectionService.ClearSelection();
        }

        public void SelectObjectivePointsInRect(Rect rect)
        {
            for (int i = 0; i < Map.ObjectivePoints.Count; i++)
            {
                var obj = Map.ObjectivePoints[i];
                if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                    ObjectivePointSelectionService.SelectAndMakeLastSelected(obj);
            }
        }

        public void SelectObjectivePoint(object obj, bool ctrlPressed)
        {
            if (obj is ObjectivePoint objectivePoint)
            {
                if (ctrlPressed)
                {
                    ObjectivePointSelectionService.CtrlSelect(objectivePoint);
                }
                else
                {
                    ObjectivePointSelectionService.ClearSelection();
                    ObjectivePointSelectionService.SelectAndMakeLastSelected(objectivePoint);
                }
            }
        }

        public void RemoveSelectedObjectivePointsFromMap()
        {
            var selectedItems = ObjectivePointSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.ObjectivePoints.Remove(item);
            }
            ObjectivePointSelectionService.ClearSelection();
        }

        public void CreateObjectivePoint(double x, double y, double z = 0)
        {
            var wot = new ObjectivePoint(Map, NamedObject.GenerateName("ObjectivePoint", Map.ObjectivePoints), x, y, z);
            Map.ObjectivePoints.Add(wot);
            ObjectivePointSelectionService.SelectAndMakeLastSelected(wot);
        }

        public void SetAllObjectivePointsVisibility(bool isVisible)
        {
            foreach (var obj in Map.ObjectivePoints)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion

        #region MapTextPoint

        public void ActivateMapTextPoints()
        {
            copyPasteService.ClearClipboard();
            currenSelectionKBShortcutService = mapTextPointSelectionKBShortcutService;
            currentMultiMovableMapObject = MultiMapTextPointViewModel;
            currentMultiRotatableMapObject = null;
            IsMovableSelection3D = true;
            CanChangeRotationMode = false;
            ClearTransformMapCommands();
        }

        public void ClearMapTextPointSelection()
        {
            MapTextPointSelectionService.ClearSelection();
        }

        public void SelectMapTextPointsInRect(Rect rect)
        {
            for (int i = 0; i < Map.MapTextPoints.Count; i++)
            {
                var obj = Map.MapTextPoints[i];
                if (rect.Contains(new Point(obj.X, -obj.Y)) && obj.IsShownOnUi)
                    MapTextPointSelectionService.SelectAndMakeLastSelected(obj);
            }
        }

        public void SelectMapTextPoint(object obj, bool ctrlPressed)
        {
            if (obj is MapTextPoint mapTextPoint)
            {
                if (ctrlPressed)
                {
                    MapTextPointSelectionService.CtrlSelect(mapTextPoint);
                }
                else
                {
                    MapTextPointSelectionService.ClearSelection();
                    MapTextPointSelectionService.SelectAndMakeLastSelected(mapTextPoint);
                }
            }
        }

        public void RemoveSelectedMapTextPointsFromMap()
        {
            var selectedItems = MapTextPointSelectionService.SelectedMapObjects.ToArray();
            foreach (var item in selectedItems)
            {
                Map.MapTextPoints.Remove(item);
            }
            MapTextPointSelectionService.ClearSelection();
        }

        public void CreateMapTextPoint(double x, double y, double z = 0)
        {
            var wot = new MapTextPoint(Map, NamedObject.GenerateName("MapTextPoint", Map.MapTextPoints), StringDictionnary.MapTextItems.Keys.ElementAtOrDefault(SelectedMapTextPointPreviewTextIndex), x, y, z);
            Map.MapTextPoints.Add(wot);
            MapTextPointSelectionService.SelectAndMakeLastSelected(wot);
        }

        public void SetAllMapTextPointsVisibility(bool isVisible)
        {
            foreach (var obj in Map.MapTextPoints)
            {
                obj.IsShownOnUi = isVisible;
            }
        }

        #endregion
    }
}
