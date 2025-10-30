using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public IList<WorldObject> WorldObjects { get; }
        public IList<Team> Teams { get; }
        public IList<Player> Players { get; }
        public IList<Group> Groups { get; }
        public IList<WaypointPath> WaypointPaths { get; }
        public IList<WorldPolygon> WorldPolygons { get; }
        public IList<WorldPoint> WorldPoints { get; }
        public IList<Flag> Flags { get; }
        public IList<PlayerAlliance> PlayerAlliances { get; }
        public IList<Timer> Timers { get; }
        public IList<SpeechEvent> SpeechEvents { get; }
        public IList<WorldRule> WorldRules { get; }
        public IList<ShipUnit> ShipUnits { get; }
        public IList<ObjectivePoint> ObjectivePoints { get; }
        public IList<ObjectiveTask> ObjectiveTasks { get; }
        public IList<MapTextPoint> MapTextPoints { get; }
        public IList<JournalEntry> JournalEntries { get; }

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
