using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPMapEditor.Data;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.ViewModel
{
    public partial class ImportMapViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool importMapSize, importWorldInfo;
        [ObservableProperty]
        private bool importWorldObjects, importSelectableTeams, importInGameTeams, importPlayers, importGroups, importWaypointPaths, importWorldPolygons, importWorldPointSets, importFlags, importPlayerAlliances, importTimers;
        [ObservableProperty]
        private bool importSpeechEvents, importWorldRules, importObjectivePoints, importObjectiveTasks, importMapTextPoints, importJournalEntries, importWorldCrews, importWorldArms;

        private readonly WorldMap map;

        public ImportMapViewModel(WorldMap map, bool checkAll = false)
        {
            this.map = map;
            if (checkAll)
                OnCheckAll();
        }

        [RelayCommand]
        private void OnCheckAll()
        {
            ImportMapSize = ImportWorldInfo = true;
            ImportWorldObjects = ImportSelectableTeams = ImportInGameTeams = ImportPlayers = ImportGroups = ImportWaypointPaths = ImportWorldPolygons = ImportWorldPointSets = ImportFlags = ImportPlayerAlliances = ImportTimers = true;
            ImportSpeechEvents = ImportWorldRules = ImportObjectivePoints = ImportObjectiveTasks = ImportMapTextPoints = ImportJournalEntries = ImportWorldCrews = ImportWorldArms = true;
        }

        [RelayCommand]
        private void OnUncheckAll()
        {
            ImportMapSize = ImportWorldInfo = false;
            ImportWorldObjects = ImportSelectableTeams = ImportInGameTeams = ImportPlayers = ImportGroups = ImportWaypointPaths = ImportWorldPolygons = ImportWorldPointSets = ImportFlags = ImportPlayerAlliances = ImportTimers = false;
            ImportSpeechEvents = ImportWorldRules = ImportObjectivePoints = ImportObjectiveTasks = ImportMapTextPoints = ImportJournalEntries = ImportWorldCrews = ImportWorldArms = false;
        }

        /// <summary>
        /// Adds WorldObjects, SelectableTeams, InGameTeams, Players, Groups, WaypointPaths, WorldPolygons, WorldPointSets, Flags, PlayerAlliances, Timers,
        /// SpeechEvents, WorldRules, Nebulas, EtheriumCurrents, ShipUnits, ObjectivePoints, ObjectiveTasks, MapTextPoints, JournalEntries, WorldCrews, WorldArms
        /// from a map
        /// </summary>
        /// <param name="map"></param>
        public void ImportMap(WorldMap map)
        {
            if (ImportMapSize)
            {
                this.map.Size = map.Size;
                this.map.ZSize = map.ZSize;
                this.map.WorldBuffer = map.WorldBuffer;
            }
            if (ImportWorldInfo)
            {
                this.map.MustAssembleFleet = map.MustAssembleFleet;
                this.map.IsCampaign = map.IsCampaign;
                this.map.IsAllianceChangeAllowed = map.IsAllianceChangeAllowed;
                this.map.IsMultiplayer = map.IsMultiplayer;
                this.map.IslandsMakeSounds = map.IslandsMakeSounds;
                this.map.WorldName = map.WorldName;
                this.map.UseCustomName = map.UseCustomName;
                this.map.CustomName = map.CustomName;
                this.map.UseCustomDescription = map.UseCustomDescription;
                this.map.CustomDescription = map.CustomDescription;
                this.map.Skybox = map.Skybox;
                this.map.StarmapTexture = map.StarmapTexture;
                this.map.AmbientLightColor = map.AmbientLightColor;
                this.map.RoofLightColor = map.RoofLightColor;
                this.map.FloorLightColor = map.FloorLightColor;
                this.map.RoofLightOrientationYaw = map.RoofLightOrientationYaw;
                this.map.RoofLightOrientationPitch = map.RoofLightOrientationPitch;
                this.map.CurrentObjectivePoint = map.CurrentObjectivePoint;
                this.map.IsCurrentObjectivePointVisibleOnStarMap = map.IsCurrentObjectivePointVisibleOnStarMap;
                this.map.JournalTitle = map.JournalTitle;
                this.map.JournalMusic = map.JournalMusic;
            }
            if (ImportWorldObjects)
            {
                for (int i = 0; i < map.WorldObjects.Count; i++)
                {
                    var item = map.WorldObjects[i];
                    this.map.WorldObjects.Add(item);
                }
            }
            if (ImportSelectableTeams)
            {
                for (int i = 0; i < map.SelectableTeams.Count; i++)
                {
                    var item = map.SelectableTeams[i];
                    this.map.SelectableTeams.Add(item);
                }
            }
            if (ImportInGameTeams)
            {
                for (int i = 0; i < map.InGameTeams.Count; i++)
                {
                    var item = map.InGameTeams[i];
                    this.map.InGameTeams.Add(item);
                }
            }
            if (ImportPlayers)
            {
                for (int i = 0; i < map.Players.Count; i++)
                {
                    var item = map.Players[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.Players);
                    this.map.Players.Add(item);
                }
            }
            if (ImportGroups)
            {
                for (int i = 0; i < map.Groups.Count; i++)
                {
                    var item = map.Groups[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.Groups);
                    this.map.Groups.Add(item);
                }
            }
            if (ImportWaypointPaths)
            {
                for (int i = 0; i < map.WaypointPaths.Count; i++)
                {
                    var item = map.WaypointPaths[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.WaypointPaths);
                    this.map.WaypointPaths.Add(item);
                }
            }
            if (ImportWorldPolygons)
            {
                for (int i = 0; i < map.WorldPolygons.Count; i++)
                {
                    var item = map.WorldPolygons[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.WorldPolygons);
                    this.map.WorldPolygons.Add(item);
                }
            }
            if (ImportWorldPointSets)
            {
                for (int i = 0; i < map.WorldPointSets.Count; i++)
                {
                    var item = map.WorldPointSets[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.WorldPointSets);
                    this.map.WorldPointSets.Add(item);
                }
            }
            if (ImportFlags)
            {
                for (int i = 0; i < map.Flags.Count; i++)
                {
                    var item = map.Flags[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.Flags);
                    this.map.Flags.Add(item);
                }
            }
            if (ImportPlayerAlliances)
            {
                for (int i = 0; i < map.PlayerAlliances.Count; i++)
                {
                    var item = map.PlayerAlliances[i];
                    this.map.PlayerAlliances.Add(item);
                }
            }
            if (ImportTimers)
            {
                for (int i = 0; i < map.Timers.Count; i++)
                {
                    var item = map.Timers[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.Timers);
                    this.map.Timers.Add(item);
                }
            }
            if (ImportSpeechEvents)
            {
                for (int i = 0; i < map.SpeechEvents.Count; i++)
                {
                    var item = map.SpeechEvents[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.SpeechEvents);
                    this.map.SpeechEvents.Add(item);
                }
            }
            if (ImportWorldRules && ImportWorldObjects && ImportFlags && ImportGroups && ImportMapTextPoints && ImportObjectivePoints && ImportObjectiveTasks && ImportPlayers && ImportSpeechEvents && ImportInGameTeams && ImportTimers && ImportWaypointPaths && ImportWorldPolygons && ImportWorldPointSets)
            {
                for (int i = 0; i < map.EtheriumCurrents.Count; i++)
                {
                    var item = map.EtheriumCurrents[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.EtheriumCurrents);
                    this.map.EtheriumCurrents.Add(item);
                }
                for (int i = 0; i < map.ShipUnits.Count; i++)
                {
                    var item = map.ShipUnits[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.ShipUnits);
                    this.map.ShipUnits.Add(item);
                }
                for (int i = 0; i < map.Nebulas.Count; i++)
                {
                    var item = map.Nebulas[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.Nebulas);
                    this.map.Nebulas.Add(item);
                }
                for (int i = 0; i < map.WorldRules.Count; i++)
                {
                    var item = map.WorldRules[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.WorldRules);
                    this.map.WorldRules.Add(item);
                }
            }
            if (ImportObjectivePoints)
            {
                for (int i = 0; i < map.ObjectivePoints.Count; i++)
                {
                    var item = map.ObjectivePoints[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.ObjectivePoints);
                    this.map.ObjectivePoints.Add(item);
                }
            }
            if (ImportObjectiveTasks)
            {
                for (int i = 0; i < map.ObjectiveTasks.Count; i++)
                {
                    var item = map.ObjectiveTasks[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.ObjectiveTasks);
                    this.map.ObjectiveTasks.Add(item);
                }
            }
            if (ImportMapTextPoints)
            {
                for (int i = 0; i < map.MapTextPoints.Count; i++)
                {
                    var item = map.MapTextPoints[i];
                    item.Name = NamedObject.GenerateName(item.Name, this.map.MapTextPoints);
                    this.map.MapTextPoints.Add(item);
                }
            }
            if (ImportJournalEntries)
            {
                for (int i = 0; i < map.JournalEntries.Count; i++)
                {
                    var item = map.JournalEntries[i];
                    this.map.JournalEntries.Add(item);
                }
            }
            if (ImportWorldCrews)
            {
                for (int i = 0; i < map.WorldCrews.Count; i++)
                {
                    var item = map.WorldCrews[i];
                    this.map.WorldCrews.Add(item);
                }
            }
            if (ImportWorldArms)
            {
                for (int i = 0; i < map.WorldArms.Count; i++)
                {
                    var item = map.WorldArms[i];
                    this.map.WorldArms.Add(item);
                }
            }
            this.map.ReorganizeWorldObjectIds();
            map.Reset();
        }

        partial void OnImportGroupsChanging(bool value)
        {
            if (value)
                ImportWorldObjects = true;
        }

        partial void OnImportPlayerAlliancesChanging(bool value)
        {
            if (value)
                ImportPlayers = true;
        }

        partial void OnImportWorldRulesChanging(bool value)
        {
            if (value)
                ImportWorldObjects = ImportFlags = ImportGroups = ImportMapTextPoints = ImportObjectivePoints = ImportObjectiveTasks = ImportPlayers = ImportSpeechEvents = ImportInGameTeams = ImportTimers = ImportWaypointPaths = ImportWorldPolygons = ImportWorldPointSets = true;
        }
    }
}
