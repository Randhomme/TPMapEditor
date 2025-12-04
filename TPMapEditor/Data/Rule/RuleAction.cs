using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TPMapEditor.Enums;
using TPMapEditor.Settings;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleAction : ObservableObject
    {
        private WorldMap map;
        [ObservableProperty]
        private Enums.RuleAction type;

        [ObservableProperty]
        private ShipUnit? shipUnit; //for ship unit creation/edition

        public ObservableCollection<RuleField> RuleFields { get; } = new();

        public RuleAction(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(type);
        }

        private void AddRuleFieldAiStance(string? realLabel, string? label, AiStance value = AiStance.AISTANCE, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldAiStance(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldBannerType(string? realLabel, string? label, BannerType value = BannerType.NoBanner, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBannerType(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldBool(string? realLabel, string? label, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBool(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDialogueAudio(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.DialogueFilesList.FirstOrDefault();
            RuleFields.Add(new RuleFieldDialogueAudio(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDouble(string? realLabel, string? label, double value = 0, double min = -9999, double max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldDouble(realLabel, label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldCrewSkillLevel(string? realLabel, string? label, CrewSkillLevel value = CrewSkillLevel.CREWSKILLLEVEL, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldCrewSkillLevel(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEffect(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.Effects.FirstOrDefault();
            RuleFields.Add(new RuleFieldEffect(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEquivalence(string? realLabel, string? label, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldEquivalence(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlag(string? realLabel, string? label, Flag? flag = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flag ??= map.Flags.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlag(realLabel, label, flag, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlagTexture(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.FlagTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlagTexture(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFollowMode(string? realLabel, string? label, FollowMode value = FollowMode.ToEnd, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldFollowMode(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFormationType(string? realLabel, string? label, FormationType value = FormationType.Column, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldFormationType(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroup(string? realLabel, string? label, Group? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroup(realLabel, label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroupUnit(string? realLabel, string? label, Group? selectedGroup = null, NamedElement? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.Groups.FirstOrDefault();
            selectedGroup ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroupUnit(realLabel, label, selectedGroup, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGuiTexture(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.GuiTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldGuiTexture(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInGameMessage(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault();
            RuleFields.Add(new RuleFieldInGameMessage(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInt(string? realLabel, string? label, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldInt(realLabel, label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldMapTextPoint(string? realLabel, string? label, MapTextPoint? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.MapTextPoints.FirstOrDefault();
            RuleFields.Add(new RuleFieldMapTextPoint(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldMusic(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.Musics.FirstOrDefault();
            RuleFields.Add(new RuleFieldMusic(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObjectivePoint(string? realLabel, string? label, ObjectivePoint? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.ObjectivePoints.FirstOrDefault();
            RuleFields.Add(new RuleFieldObjectivePoint(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObjectiveTask(string? realLabel, string? label, ObjectiveTask? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.ObjectiveTasks.FirstOrDefault();
            RuleFields.Add(new RuleFieldObjectiveTask(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObservableCollection(string? realLabel, string? label, ObservableCollection<RuleField> value, bool isOptional = true)
        {
            RuleFields.Add(new RuleFieldObservableCollection(realLabel, value, isOptional, label, true));
        }

        private void AddRuleFieldPath(string? realLabel, string? label, WaypointPath? path = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            path ??= map.WaypointPaths.FirstOrDefault();
            RuleFields.Add(new RuleFieldWaypointPath(realLabel, label, path, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPlayer(string? realLabel, string? label, Player? player = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            player ??= map.Players.FirstOrDefault();
            RuleFields.Add(new RuleFieldPlayer(realLabel, label, player, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPolygon(string? realLabel, string? label, WorldPolygon? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldSinglePlayerMission(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.SinglePlayerMissions.FirstOrDefault();
            RuleFields.Add(new RuleFieldSinglePlayerMission(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldShipName(string? realLabel, string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= StringDictionnary.ShipNames.Keys.FirstOrDefault();
            RuleFields.Add(new RuleFieldShipName(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldShipUnitName(string? realLabel, string? label, ShipUnit unit, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            var field = new RuleFieldShipUnitName(realLabel, label, unit, null, isOptional, optionalLabel, isShown);
            RuleFields.Add(field);
        }

        private void AddRuleFieldSpeechEvent(string? realLabel, string? label, SpeechEvent? speechEvent = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            speechEvent ??= map.SpeechEvents.FirstOrDefault();
            RuleFields.Add(new RuleFieldSpeechEvent(realLabel, label, speechEvent, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldString(string? realLabel, string? label, string value = "", bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            var field = new RuleFieldString(realLabel, label, value, isOptional, optionalLabel, isShown);
            RuleFields.Add(field);
        }

        private void AddRuleFieldTeam(string? realLabel, string? label, Team? team = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            team ??= map.InGameTeams.FirstOrDefault();
            RuleFields.Add(new RuleFieldTeam(realLabel, label, team, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTimer(string? realLabel, string? label, Timer? timer = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            timer ??= map.Timers.FirstOrDefault();
            RuleFields.Add(new RuleFieldTimer(realLabel, label, timer, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldUnit(string? realLabel, string? label, Group? selectedGroup = null, ShipUnit? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            selectedGroup ??= map.Groups.FirstOrDefault();
            value ??= selectedGroup.ShipUnits.FirstOrDefault();
            RuleFields.Add(new RuleFieldUnit(realLabel, label, selectedGroup, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVitalSection(string? realLabel, string? label, VitalSection vitalSection = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldVitalSection(realLabel, label, vitalSection, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVolume(string? realLabel, string? label, WorldPolygon? volume = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            volume ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(realLabel, label, volume, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldObject(string? realLabel, string? label, WorldObject? worldObject = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true, Action<object, PropertyChangedEventArgs>? propertyChanged = null)
        {
            worldObject ??= map.WorldObjects.FirstOrDefault();
            var field = new RuleFieldWorldObject(realLabel, label, worldObject, isOptional, optionalLabel, isShown);
            if (propertyChanged != null)
            {
                field.PropertyChanged += new PropertyChangedEventHandler(propertyChanged);
            }
            RuleFields.Add(field);
        }

        private void AddRuleFieldWorldObjectType(string? realLabel, string? label, KillableWorldObjectType worldObjectType = KillableWorldObjectType.Ship, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldWorldObjectType(realLabel, label, worldObjectType, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldPointSet(string? realLabel, string? label, WorldPointSet? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.WorldPointSets.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPointSet(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        partial void OnTypeChanged(Enums.RuleAction value)
        {
            switch (value)
            {
                case Enums.RuleAction.StateInitSetupEtheriumCurrent:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World Object ID Int", "World object");
                    AddRuleFieldPath("Etherium Path String", "Etherium current path");
                    AddRuleFieldString("Etherium Name String", "Etherium current name");
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World Object ID Int", "World object");
                    AddRuleFieldInt("Combat Strength Int", "Combat strength", min: 0);
                    AddRuleFieldPlayer("Player/Owner String", "Player/Owner", isOptional: true, optionalLabel: "Has player/owner");
                    AddRuleFieldCrewSkillLevel("Gunnery Level String", "Gunnery level");
                    AddRuleFieldAiStance("AI Stance String", "Ai stance");
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World Object ID Int", "World object");
                    AddRuleFieldString("New Nebula Name String", "Nebula name");
                    AddRuleFieldPolygon("Polygon Name String", "Polygon");
                    AddRuleFieldObservableCollection("Lightning On/Off String", "Has lightning", new()
                    {
                        new RuleFieldDouble("Lightning Blast Recharge Time Float", "Lightning blast recharge time", min: 0)
                    });
                    AddRuleFieldObservableCollection("Meteors On/Off String", "Has meteors", new()
                    {
                        new RuleFieldDouble("Meteor Strike Recharge Time Float", "Meteors strike recharge time", min: 0)
                    });
                    AddRuleFieldEffect("Nebula Cloud Effect Name String", "Nebula cloud effect", AppSettings.Effects.FirstOrDefault());
                    AddRuleFieldEffect("Solar Storm Effect Name String", "Solar storm effect", AppSettings.Effects.FirstOrDefault());
                    AddRuleFieldEffect("Meteor Shower Effect Name  String", "Meteor shower effect", AppSettings.Effects.FirstOrDefault());
                    AddRuleFieldWorldPointSet("Nebula Cloud Point Set Name String", "Nebula cloud point set", map.WorldPointSets.FirstOrDefault(), true, "Has nebula cloud point set");
                    AddRuleFieldWorldPointSet("Solar Storm Point Set Name String", "Solar storm point set", map.WorldPointSets.FirstOrDefault(), true, "Has solar storm point set");
                    AddRuleFieldWorldPointSet("Meteor Shower Point Set Name String", "Meteor shower point set", map.WorldPointSets.FirstOrDefault(), true, "Has meteor shower point set");
                    AddRuleFieldObservableCollection("Rotational Winds On/Off String", "Has rotational winds", new()
                    {
                        new RuleFieldDouble("Wind Magnitude Float", "Wind magnitude"), //might need to be >= 0
                    });
                    AddRuleFieldBool("Nebula Cloud Energy Drain On/Off String", "Nebula cloud eneergy drain");
                    AddRuleFieldBool("Nebula Occlusion On/Off String", "Nebula occlusion");
                    AddRuleFieldDouble("Solar Storm Wind Damage Frequency Float", "Wind damage frequency"); //might need to be >= 0
                    AddRuleFieldDouble("Ambient sound max distance Float", "Ambient sound max distance");
                    break;
                case Enums.RuleAction.StateInitSetupShip:
                    RuleFields.Clear();
                    ShipUnit = new(map, NamedElement.GenerateName("Ship", map.ShipUnits));
                    map.ShipUnits.Add(ShipUnit);
                    AddRuleFieldWorldObject("World Object ID Int", "World object", ShipUnit.WorldObject, propertyChanged: (s, e) =>
                    {
                        if (s is RuleFieldWorldObject rfwo && e.PropertyName == "Value")
                        {
                            ShipUnit.WorldObject = rfwo.Value;
                        }
                    });
                    AddRuleFieldShipUnitName("Ship Name String", "Ship name", ShipUnit);
                    AddRuleFieldPath("Ship Path String", "Ship path", isOptional: true, optionalLabel: "Has path");
                    AddRuleFieldFollowMode("Follow Mode String", "Follow mode");
                    AddRuleFieldAiStance("AI Stance String", "AI stance");
                    AddRuleFieldPlayer("Player/Owner String", "Player/Owner", isOptional: true, optionalLabel: "Has player/owner");
                    AddRuleFieldBool("Primary Ship String", "Primary ship");
                    AddRuleFieldCrewSkillLevel("Crew Skill Level String", "Crew skill level");
                    AddRuleFieldBool("Boardable String", "Boardable");
                    AddRuleFieldShipName("Localized Ship Name String", "Localized ship name");
                    break;
                case Enums.RuleAction.AddVictoryPointsForSinglePlayer:
                    RuleFields.Clear();
                    AddRuleFieldInt("Victory points to be added Int", "Victory points to be added"); //might need to be >= 0
                    break;
                case Enums.RuleAction.BreakTow:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("TowerGroupA String", "Tower group A");
                    AddRuleFieldGroupUnit("TowerGroupB String", "Tower group B"); //should probably be unit
                    break;
                case Enums.RuleAction.ClearGroupUnitBorderZone:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.ClearAllAICommands:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.CloseHUDTextureOverlay:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.CreateReleaseEventEffect:
                    RuleFields.Clear();
                    AddRuleFieldWorldPointSet("Point Set String", "Point set");
                    AddRuleFieldEffect("Effect Name String", "Effect");
                    AddRuleFieldBool("Boolean State String", "State");
                    break;
                case Enums.RuleAction.CrewSpeechHelmOffCourse:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.CrewSpeechToggleOnOff:
                    RuleFields.Clear();
                    AddRuleFieldBool("Crew Speech State State String", "Crew speech state");
                    break;
                case Enums.RuleAction.DamageGroupUnitByXPercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldDouble("Percent Float", "Percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.DestroyGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.DockShips:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Docking Group String", "Docking group");
                    AddRuleFieldGroupUnit("Target to Dock to String", "Target to dock");
                    break;
                case Enums.RuleAction.DragonSetAIStance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Dragon group");
                    AddRuleFieldAiStance("AI Stance String", "AI stance");
                    break;
                case Enums.RuleAction.DragonSetDamageThreshold:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Dragon group");
                    AddRuleFieldDouble("Damage Threshold Float", "Damage threshold", min: 0, max: 1);
                    break;
                case Enums.RuleAction.EndGame:
                    RuleFields.Clear();
                    AddRuleFieldObservableCollection("Use Custom Message String", "Use custom message", new()
                    {
                        new RuleFieldInGameMessage("Winner - Custom Message String ID String", "Winner message", StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault()),
                        new RuleFieldInGameMessage("Loser - Custom Message String ID String", "Loser message", StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault())
                    });
                    AddRuleFieldBool("Show Stats Screen String", "Show stats screen");
                    break;
                case Enums.RuleAction.FocusCameraOnGroup:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("GroupUnit Name String", "Group/Unit");
                    AddRuleFieldDouble("Distance Float", "Distance", min: 0);
                    AddRuleFieldDouble("Relative Angle Float", "Relative angle", min: 0, max: 360);
                    AddRuleFieldBool("Use Transition String", "Use transition");
                    break;
                case Enums.RuleAction.GotoNextLevel:
                    RuleFields.Clear();
                    AddRuleFieldSinglePlayerMission("World String", "Next level");
                    AddRuleFieldBool("Display Loading String String", "Display loading string");
                    break;
                case Enums.RuleAction.GrantTeamXPoints:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldInt("Points Int", "Point"); //might need to be >= 0
                    break;
                case Enums.RuleAction.GroupToFollowPath:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldPath("Path Name String", "Path");
                    AddRuleFieldFollowMode("Follow Mode String", "Follow mode");
                    AddRuleFieldBool("Find Closest Point String", "Find closest");
                    break;
                case Enums.RuleAction.GroupAToRamGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit A");
                    AddRuleFieldGroupUnit("Group Unit Target String", "Group/Unit B");
                    break;
                case Enums.RuleAction.GroupAToAttackGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Attack Group String", "Attack group/unit");
                    AddRuleFieldGroupUnit("Target Group String", "Target group/unit");
                    break;
                case Enums.RuleAction.Mission9DoDarkMatterExplosion:
                    RuleFields.Clear();
                    AddRuleFieldPolygon("Affected Area String", "Affected area");
                    break;
                case Enums.RuleAction.Mission9TeleportLongboat:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Longboat Group Name String", "Longboat group/unit");
                    break;
                case Enums.RuleAction.NISAttachCamera:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldDouble("Distance Float", "Distance", min: 0);
                    AddRuleFieldDouble("Angle YZ Float", "Angle YZ", min: 0, max: 360);
                    AddRuleFieldDouble("Angle XY Float", "Angle XY", min: 0, max: 360);
                    break;
                case Enums.RuleAction.NISEnd:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.NISFocusCameraOnGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("GroupUnit Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.NISFocusCameraOnPoint:
                    RuleFields.Clear();
                    AddRuleFieldWorldPointSet("Point Set Name String", "Point set");
                    break;
                case Enums.RuleAction.NISFocusOnMainShip:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.NISPositionCameraRelativeToObject:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Unit Name String", "Group/Unit");
                    AddRuleFieldDouble("Distance Float", "Distance", min: 0);
                    AddRuleFieldDouble("Angle YZ Float", "Angle YZ", min: 0, max: 360);
                    AddRuleFieldDouble("Angle XY Float", "Angle XY", min: 0, max: 360);
                    AddRuleFieldBool("Jump to point String", "Jump to point");
                    break;
                case Enums.RuleAction.NISSetCameraPath:
                    RuleFields.Clear();
                    AddRuleFieldPath("Path Name String", "Path");
                    AddRuleFieldBool("Jump to start String", "Jump to start");
                    break;
                case Enums.RuleAction.NISSetCameraSpeed:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Acceleration Float", "Acceleration", min: 0);
                    AddRuleFieldDouble("Max Velocity Float", "Max velocity", min: 0);
                    break;
                case Enums.RuleAction.NISSetTransitionCameraSpeed:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Acceleration Float", "Acceleration", min: 0);
                    AddRuleFieldDouble("Max Velocity Float", "Max velocity", min: 0);
                    break;
                case Enums.RuleAction.NISStart:
                    RuleFields.Clear();
                    AddRuleFieldBool("All Objects Visible String", "All objects visible");
                    AddRuleFieldBool("Open NIS bars instantly String", "Open NIS bars instantly");
                    break;
                case Enums.RuleAction.NISToggleAllObjectsVisibility:
                    RuleFields.Clear();
                    AddRuleFieldBool("All Objects Visible String", "All objects visible");
                    break;
                case Enums.RuleAction.NISToggleNISModeGunAccuracy:
                    RuleFields.Clear();
                    AddRuleFieldBool("Use NIS Gun Accuracy String", "Use NIS gun accuracy");
                    break;
                case Enums.RuleAction.NISZoom: //to be tested (values are probably not very high)
                    RuleFields.Clear();
                    AddRuleFieldDouble("FOV Float", "FOV");
                    AddRuleFieldDouble("Speed Float", "Speed");
                    break;
                case Enums.RuleAction.OpenCrewAndArmsScreens:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.OpenHUDTextureOverlay:
                    RuleFields.Clear();
                    AddRuleFieldGuiTexture("Texture Base Name String", "Texture");
                    break;
                case Enums.RuleAction.OpenWeaponBar:
                    RuleFields.Clear();
                    AddRuleFieldBool("OpenState String", "Open state");
                    break;
                case Enums.RuleAction.PlayMusicTrack:
                    RuleFields.Clear();
                    AddRuleFieldMusic("File Name String", "Track");
                    AddRuleFieldBool("Crossfade transition String", "Crossfade transition");
                    AddRuleFieldDouble("Fade Out Time ( secs ) Float", "Fade out time (secs)", min: 0);
                    AddRuleFieldDouble("Fade In Time ( secs ) Float", "Fade in time (secs)", min: 0);
                    AddRuleFieldDouble("New Volume ( 0 to 1 ) Float", "New volume", min: 0, max: 1);
                    break;
                case Enums.RuleAction.PlaySpecialEffect:
                    RuleFields.Clear();
                    AddRuleFieldDialogueAudio("File Name String", "File name");
                    AddRuleFieldBool("Play As Dialog String", "Play as dialogue");
                    break;
                case Enums.RuleAction.PlaySpeechEvent:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech Event Name String", "Speech event");
                    break;
                case Enums.RuleAction.RemainingTeamWins:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.ResetHitCount:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.ResetShotsFiredCount:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePoint:
                    RuleFields.Clear();
                    AddRuleFieldObjectivePoint("Objective Point String", "Objective point", isOptional: true, optionalLabel: "Has objective point");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointOnShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointVisibleOnStarmap:
                    RuleFields.Clear();
                    AddRuleFieldBool("Visible On Starmap String", "Visible on starmap");
                    break;
                case Enums.RuleAction.SetDockTime:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Docking GroupUnit String", "Docking group/unit");
                    AddRuleFieldDouble("Dock Time Float", "Dock time", min: 0);
                    break;
                case Enums.RuleAction.SetFlagAction:
                    RuleFields.Clear();
                    AddRuleFieldFlag("Flag Name String", "Flag name");
                    AddRuleFieldBool("Boolean Value String", "Value");
                    break;
                case Enums.RuleAction.SetFleetHoldFire:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("PLAYER FLEET String", "Player fleet");
                    AddRuleFieldBool("HOLD FIRE String", "Hold fire");
                    break;
                case Enums.RuleAction.SetFleetHoldFormation:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player Name String", "Player fleet");
                    AddRuleFieldBool("Hold Formation String", "Hold formation");
                    break;
                case Enums.RuleAction.SetFleetPrimaryShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "New primary ship"); //should probably be unit
                    break;
                case Enums.RuleAction.SetFleetFormationType:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player Name String", "Player fleet");
                    AddRuleFieldFormationType("Formation Type String", "Formation type");
                    AddRuleFieldBool("Hold Formation String", "Hold formation");
                    break;
                case Enums.RuleAction.SetGroupSpaceObjectsVelocity:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group"); //Should probably be a group and not unit
                    AddRuleFieldInt("Velocity Int", "Velocity", min: 0); //can probably < 0, just for fun
                    break;
                case Enums.RuleAction.SetGroupThrottlePercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldDouble("Throttle Percent Float", "Throttle percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetGroupUnitAICaptain:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldAiStance("AI Stance String", "AI stance");
                    break;
                case Enums.RuleAction.SetGroupUnitAIStance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldAiStance("AI Stance String", "AI stance");
                    break;
                case Enums.RuleAction.SetGroupUnitBoardable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Boardable");
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZone:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldPolygon("Polygon Name String", "Border zone");
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZoneReturnPoint:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldWorldPointSet("Pointset Name String", "Return point");
                    break;
                case Enums.RuleAction.SetGroupUnitCloakState:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Cloak State String", "Cloak state");
                    AddRuleFieldBool("AI Can Override String", "AI can override");
                    break;
                case Enums.RuleAction.SetGroupUnitDockable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Dockable");
                    break;
                case Enums.RuleAction.SetGroupUnitHoldPosition:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Hold position");
                    break;
                case Enums.RuleAction.SetGroupUnitMissionEssential:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Mission essential");
                    break;
                case Enums.RuleAction.SetGroupUnitMovable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Movable");
                    break;
                case Enums.RuleAction.SetGroupUnitOwner:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldPlayer("New Owner String", "New owner");
                    break;
                case Enums.RuleAction.SetGroupUnitTowable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Towable");
                    break;
                case Enums.RuleAction.SetGroupUnitVisibility:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Visible");
                    break;
                case Enums.RuleAction.SetGroupUnitVulnerability:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldDouble("Invulnerable Percent Float", "Invulnerable percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetGroupUnitWarningShotMode:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("GroupUnit Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Warning shot");
                    break;
                case Enums.RuleAction.SetIsInCalyanAbyss:
                    RuleFields.Clear();
                    AddRuleFieldBool("Boolean Value String", "Caylan abyss");
                    break;
                case Enums.RuleAction.SetLifeboatCreationState:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Lifeboat");
                    break;
                case Enums.RuleAction.SetMapTextVisibility:
                    RuleFields.Clear();
                    AddRuleFieldMapTextPoint("Map Text Name String", "Map text");
                    AddRuleFieldBool("Visible On Starmap String", "Visible on starmap");
                    break;
                case Enums.RuleAction.SetMaxThrottlePercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldDouble("Throttle Percent Float", "Throttle percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetNebulaFadeoutDistance:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Fadeout Distance Float", "Fadeout distance", min: 0);
                    break;
                case Enums.RuleAction.SetNebulaToLockObjects:
                    RuleFields.Clear();
                    AddRuleFieldBool("Trap String", "Lock objects");
                    break;
                case Enums.RuleAction.SetObjectiveTaskActiveState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective Task String", "Objective task");
                    AddRuleFieldBool("Active State String", "Active state");
                    break;
                case Enums.RuleAction.SetObjectiveTaskCompleteState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective Task String", "Objective task");
                    AddRuleFieldBool("Complete State String", "Complete state");
                    break;
                case Enums.RuleAction.SetObjectiveTaskFailedState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective Task String", "Objective task");
                    AddRuleFieldBool("Failed State String", "Failed state");
                    break;
                case Enums.RuleAction.SetRadarActiveState:
                    RuleFields.Clear();
                    AddRuleFieldBool("Radar Active String", "Radar active state");
                    break;
                case Enums.RuleAction.SetShipBannerType:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldBannerType("Banner Type String", "Banner type");
                    break;
                case Enums.RuleAction.SetShipName:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldShipName("New Ship Name String", "New ship name");
                    break;
                case Enums.RuleAction.SetShipTopSpeedPercentage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldDouble("Throttle Speed Float", "Throttle speed", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetShipsFlagTexture:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldFlagTexture("Flag Texture String", "Flag texture");
                    break;
                case Enums.RuleAction.SetAllianceBetweenPlayerAToTRUEFALSE:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("PLAYER A String", "Player A");
                    AddRuleFieldPlayer("PLAYER B String", "Player B");
                    AddRuleFieldBool("SET ALLIANCE TO String", "Alliance");
                    break;
                case Enums.RuleAction.SetCollidable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldBool("Boolean String", "Collidable");
                    break;
                case Enums.RuleAction.SetupAsteroidBelt:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group Name String", "Asteroid group");
                    AddRuleFieldPath("Path Name String", "Path", isOptional: true, optionalLabel: "Has path");
                    AddRuleFieldFollowMode("Follow Mode String", "Follow mode");
                    AddRuleFieldBool("Find Closest Point String", "Find closest");
                    AddRuleFieldDouble("Velocity Upper m/sec Float", "Velocity Upper m/sec");
                    AddRuleFieldDouble("Velocity Lower m/sec Float", "Velocity Lower m/sec");
                    AddRuleFieldDouble("Tumble Upper Rads/sec Float", "Tumble Upper Rads/sec");
                    AddRuleFieldDouble("Tumble Lower Rads/sec Float", "Tumble Lower Rads/sec");
                    break;
                case Enums.RuleAction.SetupSpaceAnimalFlock:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldPath("Path Name String", "Path");
                    AddRuleFieldFollowMode("Follow Mode String", "Follow mode");
                    AddRuleFieldBool("Find Closest Point String", "Find closest");
                    AddRuleFieldInt("Velocity Int", "Velocity"); //can probably be < 0, just the fun of going reverse
                    break;
                case Enums.RuleAction.SetupTeamObjective:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldObjectivePoint("Objective Point String", "Objective point", isOptional: true, optionalLabel: "Has objective point");
                    AddRuleFieldObjectiveTask("Objective Task String", "Objective task");
                    break;
                case Enums.RuleAction.StartTimer:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer Name String", "Timer");
                    break;
                case Enums.RuleAction.StopTimer:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer Name String", "Timer");
                    break;
                case Enums.RuleAction.TutorialPauseWhenStarmapOpens:
                    RuleFields.Clear();
                    AddRuleFieldBool("Pause when starmap opens String", "Pause when star map opens");
                    break;
                case Enums.RuleAction.TeamXWins:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    break;
                case Enums.RuleAction.TeleportGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldWorldPointSet("Point Set Name String", "Point set");
                    break;
                case Enums.RuleAction.ToggleIslandRepairWhenDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group Name String", "Group");
                    AddRuleFieldBool("Repair when docked String", "Repair when docked");
                    break;
                case Enums.RuleAction.TowShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Docking Group String", "Towing group");
                    AddRuleFieldGroupUnit("Target to Tow String", "Target to tow"); //might need to be unit
                    break;
                case Enums.RuleAction.TransferGroupUnitToGroup:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit to transfer String", "Group/Unit to transfer");
                    AddRuleFieldGroup("Target Group String", "Target group");
                    break;
                default:
                    break;
            }
        }
    }
}
