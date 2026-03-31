using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Data.Rule;
using TPMapEditor.Settings;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public partial class WorldMap : CustomObservableValidator
    {
        [ObservableProperty]
        private bool mustAssembleFleet, isMultiplayer, isCampaign, useCustomName, useCustomDescription, playEndMovie, isAllianceChangeAllowed, islandsMakeSounds, isCurrentObjectivePointVisibleOnStarMap;
        [ObservableProperty]
        private int size, zSize, playerPlayableCount, roofLightOrientationYaw, roofLightOrientationPitch;
        [ObservableProperty]
        [property: Required]
        private string worldName, worldDescription, starmapTexture, skybox, journalMusic, journalTitle;
        [ObservableProperty]
        private string customName, customDescription;
        [ObservableProperty]
        private Color ambientLightColor, roofLightColor, floorLightColor;
        [ObservableProperty]
        private ObjectivePoint? currentObjectivePoint;
        [ObservableProperty]
        private double worldBuffer; // Size of the off map area

        public ObservableCollection<WorldObject> WorldObjects { get; }
        public ObservableCollection<Team> SelectableTeams { get; }
        public ObservableCollection<Team> InGameTeams { get; }
        public ObservableCollection<Player> Players { get; }
        public ObservableCollection<Group> Groups { get; }
        public ObservableCollection<WaypointPath> WaypointPaths { get; }
        public ObservableCollection<WorldPolygon> WorldPolygons { get; }
        public ObservableCollection<WorldPointSet> WorldPointSets { get; }
        public ObservableCollection<Flag> Flags { get; }
        public ObservableCollection<PlayerAlliance> PlayerAlliances { get; }
        public ObservableCollection<Timer> Timers { get; }
        public ObservableCollection<SpeechEvent> SpeechEvents { get; }
        public ObservableCollection<WorldRule> WorldRules { get; }
        public ObservableCollection<Nebula> Nebulas { get; }
        public ObservableCollection<EtheriumCurrent> EtheriumCurrents { get; }
        public ObservableCollection<ShipUnit> ShipUnits { get; }
        public ObservableCollection<ObjectivePoint> ObjectivePoints { get; }
        public ObservableCollection<ObjectiveTask> ObjectiveTasks { get; }
        public ObservableCollection<MapTextPoint> MapTextPoints { get; }
        public ObservableCollection<JournalEntry> JournalEntries { get; }
        public ObservableCollection<WorldObjectType> WorldCrews { get; }
        public ObservableCollection<WorldObjectType> WorldArms { get; }
        public ObservableCollection<WaypointPath> SelectableWaypointPaths { get; } = new() { WaypointPath.DefaultWaypointPath };
        public ObservableCollection<WorldPointSet> SelectableWorldPointSets { get; } = new() { WorldPointSet.DefaultWorldPointSet };
        public ObservableCollection<Group> SelectableGroups { get; } = new() { Group.DefaultGroup };
        public ObservableCollection<ShipUnit> SelectableShipUnits { get; } = new() { ShipUnit.DefaultShipUnit };
        public ObservableCollection<ObjectivePoint> SelectableObjectivePoints { get; } = new() { ObjectivePoint.DefaultObjectivePoint };
        public ObservableCollection<Player> SelectablePlayers { get; } = new() { Player.DefaultPlayer };

        public WorldMap()
        {
            isCampaign = playEndMovie = isAllianceChangeAllowed = false;
            isMultiplayer = mustAssembleFleet = useCustomName = useCustomDescription = islandsMakeSounds = true;
            playerPlayableCount = roofLightOrientationYaw = 0;
            roofLightOrientationPitch = 90;
            worldName = StringDictionnary.WorldNames.Keys.FirstOrDefault();
            worldDescription = StringDictionnary.WorldDescriptions.Keys.FirstOrDefault() ?? string.Empty;
            customName = "My new map";
            customDescription = "The map description.";
            skybox = AppSettings.Meshes.FirstOrDefault() ?? string.Empty;
            starmapTexture = AppSettings.GuiTextures.FirstOrDefault() ?? string.Empty;
            journalMusic = AppSettings.Musics.FirstOrDefault() ?? string.Empty;
            journalTitle = StringDictionnary.JournalTitles.Keys.FirstOrDefault() ?? string.Empty;
            size = 2500;
            zSize = 1500;
            worldBuffer = 500;
            ambientLightColor = Color.FromRgb(50, 50, 50); ;
            roofLightColor = Colors.White;
            floorLightColor = Colors.Black;
            WorldObjects = new ObservableCollection<WorldObject>();
            SelectableTeams = new ObservableCollection<Team>();
            InGameTeams = new ObservableCollection<Team>();
            Players = new ObservableCollection<Player>();
            Groups = new ObservableCollection<Group>();
            WaypointPaths = new ObservableCollection<WaypointPath>();
            WorldPolygons = new ObservableCollection<WorldPolygon>();
            WorldPointSets = new ObservableCollection<WorldPointSet>();
            Flags = new ObservableCollection<Flag>();
            PlayerAlliances = new ObservableCollection<PlayerAlliance>();
            Timers = new ObservableCollection<Timer>();
            SpeechEvents = new ObservableCollection<SpeechEvent>();
            WorldRules = new ObservableCollection<WorldRule>();
            Nebulas = new ObservableCollection<Nebula>();
            EtheriumCurrents = new ObservableCollection<EtheriumCurrent>();
            ShipUnits = new ObservableCollection<ShipUnit>();
            ObjectivePoints = new ObservableCollection<ObjectivePoint>();
            ObjectiveTasks = new ObservableCollection<ObjectiveTask>();
            MapTextPoints = new ObservableCollection<MapTextPoint>();
            JournalEntries = new ObservableCollection<JournalEntry>();
            WorldCrews = new ObservableCollection<WorldObjectType>();
            WorldArms = new ObservableCollection<WorldObjectType>();
            Flags.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(Flags, e);
            Groups.CollectionChanged += OnGroupsCollectionChanged;
            InGameTeams.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(InGameTeams, e);
            MapTextPoints.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(MapTextPoints, e);
            ObjectivePoints.CollectionChanged += OnObjectivePointsCollectionChanged;
            ObjectiveTasks.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(ObjectiveTasks, e);
            Players.CollectionChanged += OnPlayersCollectionChanged;
            ShipUnits.CollectionChanged += OnShipUnitsCollectionChanged;
            SpeechEvents.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(SpeechEvents, e);
            Timers.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(Timers, e);
            WaypointPaths.CollectionChanged += OnWaypointPathsCollectionChanged;
            WorldObjects.CollectionChanged += OnWorldObjectsCollectionChanged;
            WorldPointSets.CollectionChanged += OnWorldPointSetsCollectionChanged;
            WorldPolygons.CollectionChanged += (s, e) => NullifyRuleFieldOnRemoveItems(WorldPolygons, e);
        }

        private void NullifyRuleFieldOnRemoveItems<T>(ObservableCollection<T> itemSource, NotifyCollectionChangedEventArgs e, T? defaultItem = default)
        {
            IList? collection = null;
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                collection = e.OldItems;
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                collection = itemSource;
            }
            if (collection != null)
            {
                foreach (var rule in WorldRules)
                {
                    foreach (var condition in rule.Conditions)
                    {
                        foreach (var field in condition.RuleFields)
                        {
                            if (field is RuleField<T> ruleField)
                            {
                                if (collection.Contains(ruleField.Value))
                                    ruleField.Value = defaultItem;
                                else if (ruleField.Value == null && defaultItem != null)
                                    ruleField.Value = defaultItem;
                            }

                        }
                    }
                    foreach (var action in rule.Actions)
                    {
                        foreach (var field in action.RuleFields)
                        {
                            if (field is RuleField<T> ruleField)
                            {
                                if (collection.Contains(ruleField.Value))
                                    ruleField.Value = defaultItem;
                                else if (ruleField.Value == null && defaultItem != null)
                                    ruleField.Value = defaultItem;
                            }
                        }
                    }
                }
            }
        }

        private void OnGroupsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectableGroups.SynchronizeFrom(Groups, e, new[] { Group.DefaultGroup });
            NullifyRuleFieldOnRemoveItems(Groups, e, Group.DefaultGroup);
        }

        private void OnObjectivePointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectableObjectivePoints.SynchronizeFrom(ObjectivePoints, e, new[] { ObjectivePoint.DefaultObjectivePoint });
            NullifyRuleFieldOnRemoveItems(ObjectivePoints, e, ObjectivePoint.DefaultObjectivePoint);
        }

        private void OnPlayersCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Player p in e.OldItems)
                {
                    if (p.IsPlayable)
                        PlayerPlayableCount--;
                }
            }
            SelectablePlayers.SynchronizeFrom(Players, e, new[] { Player.DefaultPlayer });
            NullifyRuleFieldOnRemoveItems(Players, e, Player.DefaultPlayer);
        }

        private void OnShipUnitsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectableShipUnits.SynchronizeFrom(ShipUnits, e, new[] { ShipUnit.DefaultShipUnit });
            NullifyRuleFieldOnRemoveItems(ShipUnits, e, ShipUnit.DefaultShipUnit);
        }

        private void OnWaypointPathsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectableWaypointPaths.SynchronizeFrom(WaypointPaths, e, new[] { WaypointPath.DefaultWaypointPath });
            NullifyRuleFieldOnRemoveItems(WaypointPaths, e, WaypointPath.DefaultWaypointPath);
        }

        private void OnWorldObjectsCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (WorldObject item in e.OldItems)
                {
                    item.Group = null;
                }
            }
            NullifyRuleFieldOnRemoveItems(WorldObjects, e);
        }

        private void OnWorldPointSetsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectableWorldPointSets.SynchronizeFrom(WorldPointSets, e, new[] { WorldPointSet.DefaultWorldPointSet });
            NullifyRuleFieldOnRemoveItems(WorldPointSets, e, WorldPointSet.DefaultWorldPointSet);
        }

        public void EnableCollectionSynchronization(object _lock)
        {
            BindingOperations.EnableCollectionSynchronization(EtheriumCurrents, _lock);
            BindingOperations.EnableCollectionSynchronization(Flags, _lock);
            BindingOperations.EnableCollectionSynchronization(Groups, _lock);
            BindingOperations.EnableCollectionSynchronization(JournalEntries, _lock);
            BindingOperations.EnableCollectionSynchronization(InGameTeams, _lock);
            BindingOperations.EnableCollectionSynchronization(MapTextPoints, _lock);
            BindingOperations.EnableCollectionSynchronization(Nebulas, _lock);
            BindingOperations.EnableCollectionSynchronization(ObjectivePoints, _lock);
            BindingOperations.EnableCollectionSynchronization(ObjectiveTasks, _lock);
            BindingOperations.EnableCollectionSynchronization(PlayerAlliances, _lock);
            BindingOperations.EnableCollectionSynchronization(Players, _lock);
            BindingOperations.EnableCollectionSynchronization(SelectableTeams, _lock);
            BindingOperations.EnableCollectionSynchronization(ShipUnits, _lock);
            BindingOperations.EnableCollectionSynchronization(SpeechEvents, _lock);
            BindingOperations.EnableCollectionSynchronization(Timers, _lock);
            BindingOperations.EnableCollectionSynchronization(WaypointPaths, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldObjects, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldPointSets, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldPolygons, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldRules, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldCrews, _lock);
            BindingOperations.EnableCollectionSynchronization(WorldArms, _lock);
        }

        public void DisableCollectionSynchronization()
        {
            BindingOperations.DisableCollectionSynchronization(EtheriumCurrents);
            BindingOperations.DisableCollectionSynchronization(Flags);
            BindingOperations.DisableCollectionSynchronization(Groups);
            BindingOperations.DisableCollectionSynchronization(InGameTeams);
            BindingOperations.DisableCollectionSynchronization(JournalEntries);
            BindingOperations.DisableCollectionSynchronization(MapTextPoints);
            BindingOperations.DisableCollectionSynchronization(Nebulas);
            BindingOperations.DisableCollectionSynchronization(ObjectivePoints);
            BindingOperations.DisableCollectionSynchronization(ObjectiveTasks);
            BindingOperations.DisableCollectionSynchronization(PlayerAlliances);
            BindingOperations.DisableCollectionSynchronization(Players);
            BindingOperations.DisableCollectionSynchronization(SelectableTeams);
            BindingOperations.DisableCollectionSynchronization(ShipUnits);
            BindingOperations.DisableCollectionSynchronization(SpeechEvents);
            BindingOperations.DisableCollectionSynchronization(Timers);
            BindingOperations.DisableCollectionSynchronization(WaypointPaths);
            BindingOperations.DisableCollectionSynchronization(WorldObjects);
            BindingOperations.DisableCollectionSynchronization(WorldPointSets);
            BindingOperations.DisableCollectionSynchronization(WorldPolygons);
            BindingOperations.DisableCollectionSynchronization(WorldRules);
            BindingOperations.DisableCollectionSynchronization(WorldCrews);
            BindingOperations.DisableCollectionSynchronization(WorldArms);
        }

        public void ReorganizeWorldObjectIds()
        {
            WorldObject.ResetNextId();
            for (int i = 0; i < WorldObjects.Count; i++)
            {
                var worlObject = WorldObjects[i];
                worlObject.Id = i;
            }
        }

        public void Reset()
        {
            Clear();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            IsCampaign = PlayEndMovie = IsAllianceChangeAllowed = false;
            IsMultiplayer = MustAssembleFleet = UseCustomName = UseCustomDescription = IslandsMakeSounds = true;
            PlayerPlayableCount = RoofLightOrientationYaw = 0;
            RoofLightOrientationPitch = 90;
            WorldName = StringDictionnary.WorldNames.Keys.FirstOrDefault();
            WorldDescription = StringDictionnary.WorldDescriptions.Keys.FirstOrDefault();
            CustomName = "My new map";
            CustomDescription = "The map description.";
            Skybox = AppSettings.Meshes.FirstOrDefault();
            StarmapTexture = AppSettings.GuiTextures.FirstOrDefault();
            JournalMusic = AppSettings.Musics.FirstOrDefault();
            JournalTitle = StringDictionnary.JournalTitles.Keys.FirstOrDefault();
            Size = 2500;
            ZSize = 1500;
            WorldBuffer = 500;
            AmbientLightColor = Color.FromRgb(50,50,50);
            RoofLightColor = Colors.White;
            FloorLightColor = Colors.Black;
        }

        private void Clear()
        {
            WorldRules.Clear();
            EtheriumCurrents.Clear();
            Flags.Clear();
            Groups.Clear();
            InGameTeams.Clear();
            JournalEntries.Clear();
            MapTextPoints.Clear();
            Nebulas.Clear();
            ObjectivePoints.Clear();
            ObjectiveTasks.Clear();
            PlayerAlliances.Clear();
            Players.Clear();
            SelectableTeams.Clear();
            ShipUnits.Clear();
            SpeechEvents.Clear();
            Timers.Clear();
            WaypointPaths.Clear();
            WorldObjects.Clear();
            WorldPointSets.Clear();
            WorldPolygons.Clear();
            WorldCrews.Clear();
            WorldArms.Clear();
            WorldObject.ResetNextId();
        }
    }
}
