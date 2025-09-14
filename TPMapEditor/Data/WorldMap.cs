using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class WorldMap : ObservableObject
    {
        [ObservableProperty]
        private int size;
        [ObservableProperty]
        private bool mustAssembleFleet;
        [ObservableProperty]
        private bool isCampaign;
        [ObservableProperty]
        private int playerPlayableCount;

        private string customName;

        public string CustomName
        {
            get { return customName; }
            set 
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Value cannot be empty");
                customName = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private string customDescription;

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
        //public IList<EtheriumCurrent> EtheriumCurrents { get; }

        public WorldMap()
        {
            mustAssembleFleet = true;
            isCampaign = false;
            customName = "My new map";
            customDescription = "The map description.";
            size = 3500;
            WorldObjects = new List<WorldObject>();
            Teams = new ObservableCollection<Team>();
            Players = new ObservableCollection<Player>();
            Groups = new ObservableCollection<Group>();
            WaypointPaths = new List<WaypointPath>();
            WorldPolygons = new List<WorldPolygon>();
            WorldPoints = new List<WorldPoint>();
            Flags = new ObservableCollection<Flag>();
            PlayerAlliances = new ObservableCollection<PlayerAlliance>();
            Timers = new ObservableCollection<Timer>();
            SpeechEvents = new ObservableCollection<SpeechEvent>();
            WorldRules = new ObservableCollection<WorldRule>();
            ShipUnits = new ObservableCollection<ShipUnit>();
            //EtheriumCurrents = new ObservableCollection<EtheriumCurrent>();
        }
    }
}
