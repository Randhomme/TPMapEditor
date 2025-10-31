using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class WorldMap : ObservableObject
    {
        public static Dictionary<string, string> InGameMessagesDictionnary = new Dictionary<string, string>();

        [ObservableProperty]
        private bool mustAssembleFleet;
        [ObservableProperty]
        private bool isMultiplayer, isCampaign, playEndMovie, isAllianceChangeAllowed, islandsMakeSounds;
        [ObservableProperty]
        private int size, zSize, playerPlayableCount, roofLightOrientationYaw, roofLightOrientationPitch;
        [ObservableProperty]
        private string worldName, worldDescription, customName, customDescription, starmapTexture, skybox, journalMusic;
        [ObservableProperty]
        private Color ambientLightColor = Colors.Khaki, roofLightColor = Colors.DarkKhaki, floorLightColor = Colors.DarkKhaki;

        public ObservableCollection<WorldObject> WorldObjects { get; }
        public ObservableCollection<Team> Teams { get; }
        public ObservableCollection<Player> Players { get; }
        public ObservableCollection<Group> Groups { get; }
        public ObservableCollection<WaypointPath> WaypointPaths { get; }
        public ObservableCollection<WorldPolygon> WorldPolygons { get; }
        public ObservableCollection<WorldPoint> WorldPoints { get; }
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

        public WorldMap()
        {
            mustAssembleFleet = true;
            isCampaign = false;
            worldName = StringDictionnary.WorldNames.Keys.FirstOrDefault();
            worldDescription = StringDictionnary.WorldDescriptions.Keys.FirstOrDefault();
            customName = customDescription = string.Empty;
            skybox = AppSettings.Meshes.FirstOrDefault();
            starmapTexture = AppSettings.GuiTextures.FirstOrDefault();
            journalMusic = AppSettings.Musics.FirstOrDefault();
            size = 2500;
            zSize = 1500;
            WorldObjects = new ObservableCollection<WorldObject>();
            Teams = new ObservableCollection<Team>();
            Players = new ObservableCollection<Player>();
            Groups = new ObservableCollection<Group>();
            WaypointPaths = new ObservableCollection<WaypointPath>();
            WorldPolygons = new ObservableCollection<WorldPolygon>();
            WorldPoints = new ObservableCollection<WorldPoint>();
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
            Groups.Add(new(this, "Player0 Group") { CanBeRemoved = false });
            ShipUnits.Add(new(this, "HUMAN CONTROLLED COMMAND SHIP"));

            WaypointPaths.CollectionChanged += OnWaypointPathsChanged;
        }

        private void OnWaypointPathsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WaypointPath path in e.NewItems)
                {
                    path.Points.CollectionChanged += (_, __) => OnPointsChanged(path);
                }
            }

            if (e.OldItems != null)
            {
                foreach (WaypointPath path in e.OldItems)
                {
                    path.Points.CollectionChanged -= (_, __) => OnPointsChanged(path);
                }
            }
        }

        private void OnPointsChanged(WaypointPath path)
        {
            // S’il n’y a plus aucun point, on retire le path
            if (path.Points.Count == 0)
            {
                WaypointPaths.Remove(path);
            }
        }

        public void Reset()
        {
            Clear();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            MustAssembleFleet = true;
            IsCampaign = false;
            CustomName = "My new map";
            CustomDescription = "The map description.";
            Skybox = AppSettings.Meshes.FirstOrDefault();
            StarmapTexture = AppSettings.GuiTextures.FirstOrDefault();
            JournalMusic = AppSettings.Musics.FirstOrDefault();
            Size = 3500;
            Groups.Add(new(this, "Player0 Group") { CanBeRemoved = false });
            ShipUnits.Add(new(this, "HUMAN CONTROLLED COMMAND SHIP"));
        }

        private void Clear()
        {
            WorldObjects.Clear();
            Teams.Clear();
            Players.Clear();
            Groups.Clear();
            WaypointPaths.Clear();
            WorldPolygons.Clear();
            WorldPoints.Clear();
            Flags.Clear();
            PlayerAlliances.Clear();
            Timers.Clear();
            SpeechEvents.Clear();
            WorldRules.Clear();
            ShipUnits.Clear();
            ObjectivePoints.Clear();
            ObjectiveTasks.Clear();
            MapTextPoints.Clear();
            JournalEntries.Clear();
        }
    }
}
