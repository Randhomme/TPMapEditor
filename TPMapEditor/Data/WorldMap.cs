using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class WorldMap : ObservableObject
    {
        [ObservableProperty]
        private bool mustAssembleFleet, isMultiplayer, isCampaign, useCustomName, useCustomDescription, playEndMovie, isAllianceChangeAllowed, islandsMakeSounds;
        [ObservableProperty]
        private int size, zSize, playerPlayableCount, roofLightOrientationYaw, roofLightOrientationPitch;
        [ObservableProperty]
        private string worldName, worldDescription, customName, customDescription, starmapTexture, skybox, journalMusic, journalTitle;
        [ObservableProperty]
        private Color ambientLightColor, roofLightColor, floorLightColor;

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
        public ObservableCollection<ShipUnit> ShipUnits { get; }
        public ObservableCollection<ObjectivePoint> ObjectivePoints { get; }
        public ObservableCollection<ObjectiveTask> ObjectiveTasks { get; }
        public ObservableCollection<MapTextPoint> MapTextPoints { get; }
        public ObservableCollection<JournalEntry> JournalEntries { get; }
        public ObservableCollection<WotGridItem> WorldCrews { get; }
        public ObservableCollection<WotGridItem> WorldArms { get; }

        public WorldMap()
        {
            isCampaign = playEndMovie = isAllianceChangeAllowed = false;
            isMultiplayer = mustAssembleFleet = useCustomName = useCustomDescription = islandsMakeSounds = true;
            playerPlayableCount = roofLightOrientationYaw = roofLightOrientationPitch = 0;
            worldName = StringDictionnary.WorldNames.Keys.FirstOrDefault();
            worldDescription = StringDictionnary.WorldDescriptions.Keys.FirstOrDefault();
            customName = "My new map";
            customDescription = "The map description.";
            skybox = AppSettings.Meshes.FirstOrDefault();
            starmapTexture = AppSettings.GuiTextures.FirstOrDefault();
            journalMusic = AppSettings.Musics.FirstOrDefault();
            journalTitle = StringDictionnary.JournalTitles.Keys.FirstOrDefault();
            size = 2500;
            zSize = 1500;
            ambientLightColor = Colors.Khaki;
            roofLightColor = Colors.DarkKhaki;
            floorLightColor = Colors.DarkKhaki;
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
            ShipUnits = new ObservableCollection<ShipUnit>();
            ObjectivePoints = new ObservableCollection<ObjectivePoint>();
            ObjectiveTasks = new ObservableCollection<ObjectiveTask>();
            MapTextPoints = new ObservableCollection<MapTextPoint>();
            JournalEntries = new ObservableCollection<JournalEntry>();
            WorldCrews = new ObservableCollection<WotGridItem>();
            WorldArms = new ObservableCollection<WotGridItem>();
            Groups.Add(new(this, "Player0 Group") { CanBeRemoved = false });
            ShipUnits.Add(new(this, "HUMAN CONTROLLED COMMAND SHIP"));
            Players.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach(Player p in e.OldItems)
                    {
                        if (p.IsPlayable)
                            PlayerPlayableCount--;
                    }
                }
            };
        }

        public void EnableCollectionSynchronization(object _lock)
        {
            BindingOperations.EnableCollectionSynchronization(InGameTeams, _lock);
            BindingOperations.EnableCollectionSynchronization(Flags, _lock);
            BindingOperations.EnableCollectionSynchronization(Groups, _lock);
            BindingOperations.EnableCollectionSynchronization(JournalEntries, _lock);
            BindingOperations.EnableCollectionSynchronization(MapTextPoints, _lock);
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
            BindingOperations.DisableCollectionSynchronization(InGameTeams);
            BindingOperations.DisableCollectionSynchronization(Flags);
            BindingOperations.DisableCollectionSynchronization(Groups);
            BindingOperations.DisableCollectionSynchronization(JournalEntries);
            BindingOperations.DisableCollectionSynchronization(MapTextPoints);
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
            PlayerPlayableCount = RoofLightOrientationYaw = RoofLightOrientationPitch = 0;
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
            AmbientLightColor = Colors.Khaki;
            RoofLightColor = Colors.DarkKhaki;
            FloorLightColor = Colors.DarkKhaki;
            Groups.Add(new(this, "Player0 Group") { CanBeRemoved = false });
            ShipUnits.Add(new(this, "HUMAN CONTROLLED COMMAND SHIP"));
        }

        private void Clear()
        {
            WorldRules.Clear();
            Flags.Clear();
            Groups.Clear();
            InGameTeams.Clear();
            JournalEntries.Clear();
            MapTextPoints.Clear();
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
            WorldObject.ResetNextId();
        }
    }
}
