using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
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
            OnTypeChanged(Enums.RuleAction.StateInitSetupEtheriumCurrent);
        }

        private void AddRuleFieldAiStance(string? label = null, AiStance value = AiStance.AISTANCE, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldAiStance(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldBannerType(string? label = null, BannerType value = BannerType.NoBanner, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBannerType(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldBool(string? label = null, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBool(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDialogueAudio(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= SpeechEvent.DialogueFilesList.FirstOrDefault();
            RuleFields.Add(new RuleFieldDialogueAudio(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDouble(string? label = null, double value = 0, double min = -9999, double max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldDouble(label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldCrewSkillLevel(string? label = null, CrewSkillLevel value = CrewSkillLevel.CREWSKILLLEVEL, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldCrewSkillLevel(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEffect(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.Effects.FirstOrDefault();
            RuleFields.Add(new RuleFieldEffect(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEquivalence(string? label = null, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldEquivalence(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlag(string? label, Flag? flag = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flag ??= map.Flags.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlag(label, flag, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlagTexture(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.FlagTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlagTexture(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFollowMode(string? label = null, FollowMode value = FollowMode.ToEnd, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldFollowMode(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFormationType(string? label = null, FormationType value = FormationType.Column, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldFormationType(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroup(string? label, Group? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroup(label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroupUnit(string? label = null, NamedElement? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroupUnit(label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGuiTexture(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.GuiTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldGuiTexture(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInGameMessage(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault();
            RuleFields.Add(new RuleFieldInGameMessage(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInt(string? label = null, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldInt(label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldMapTextPoint(string? label, MapTextPoint? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.MapTextPoints.FirstOrDefault();
            RuleFields.Add(new RuleFieldMapTextPoint(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldMusic(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.Musics.FirstOrDefault();
            RuleFields.Add(new RuleFieldMusic(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObjectivePoint(string? label, ObjectivePoint? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.ObjectivePoints.FirstOrDefault();
            RuleFields.Add(new RuleFieldObjectivePoint(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObjectiveTask(string? label, ObjectiveTask? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.ObjectiveTasks.FirstOrDefault();
            RuleFields.Add(new RuleFieldObjectiveTask(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldObservableCollection(string? label, ObservableCollection<RuleField> value, bool isOptional = true)
        {
            RuleFields.Add(new RuleFieldObservableCollection(null, value, isOptional, label, true));
        }

        private void AddRuleFieldPath(string? label = null, WaypointPath? path = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            path ??= map.WaypointPaths.FirstOrDefault();
            RuleFields.Add(new RuleFieldWaypointPath(label, path, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPlayer(string? label = null, Player? player = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            player ??= map.Players.FirstOrDefault();
            RuleFields.Add(new RuleFieldPlayer(label, player, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPolygon(string? label = null, WorldPolygon? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldSinglePlayerMission(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= AppSettings.SinglePlayerMissions.FirstOrDefault();
            RuleFields.Add(new RuleFieldSinglePlayerMission(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldShipName(string? label, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= StringDictionnary.ShipNames.Keys.FirstOrDefault();
            RuleFields.Add(new RuleFieldShipName(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldSpeechEvent(string? label = null, SpeechEvent? speechEvent = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            speechEvent ??= map.SpeechEvents.FirstOrDefault();
            RuleFields.Add(new RuleFieldSpeechEvent(label, speechEvent, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldString(string? label, string value = "", bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldString(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTeam(string? label = null, Team? team = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            team ??= map.InGameTeams.FirstOrDefault();
            RuleFields.Add(new RuleFieldTeam(label, team, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTimer(string? label = null, Timer? timer = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            timer ??= map.Timers.FirstOrDefault();
            RuleFields.Add(new RuleFieldTimer(label, timer, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVitalSection(string? label = null, VitalSection vitalSection = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldVitalSection(label, vitalSection, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVolume(string? label = null, WorldPolygon? volume = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            volume ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(label, volume, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldObject(string? label = null, WorldObject? worldObject = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            worldObject ??= map.WorldObjects.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldObject(label, worldObject, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldObjectType(string? label = null, WorldObjectType worldObjectType = WorldObjectType.Ship, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldWorldObjectType(label, worldObjectType, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldPointSet(string? label = null, WorldPointSet? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            value ??= map.WorldPointSets.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPointSet(label, value, isOptional, optionalLabel, isShown));
        }

        partial void OnTypeChanged(Enums.RuleAction value)
        {
            switch (value)
            {
                case Enums.RuleAction.StateInitSetupEtheriumCurrent:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World object");
                    AddRuleFieldPath("Etherium current path");
                    AddRuleFieldString("Etherium current name");
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World object");
                    AddRuleFieldPlayer("Player/Owner", isOptional: true, optionalLabel: "Has player/owner");
                    AddRuleFieldInt("Combat strength", min: 0);
                    AddRuleFieldCrewSkillLevel("Gunnery level");
                    AddRuleFieldAiStance("Ai stance");
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    RuleFields.Clear();
                    AddRuleFieldWorldObject("World object");
                    AddRuleFieldString("Nebula name");
                    AddRuleFieldPolygon("Polygon");
                    AddRuleFieldObservableCollection("Has lightning", new()
                    {
                        new RuleFieldDouble("Lightning blast recharge time", min: 0)
                    });
                    AddRuleFieldObservableCollection("Has meteors", new()
                    {
                        new RuleFieldDouble("Meteors strike recharge time", min: 0)
                    });
                    AddRuleFieldObservableCollection("Has nebula cloud effect", new()
                    {
                        new RuleFieldEffect("Nebula cloud effect", AppSettings.Effects.FirstOrDefault()),
                        new RuleFieldWorldPointSet("Nebula cloud point set", map.WorldPointSets.FirstOrDefault())
                    });
                    AddRuleFieldObservableCollection("Has solar storm effect", new()
                    {
                        new RuleFieldEffect("Solar storm effect", AppSettings.Effects.FirstOrDefault()),
                        new RuleFieldWorldPointSet("Solar storm point set", map.WorldPointSets.FirstOrDefault())
                    });
                    AddRuleFieldObservableCollection("Has meteor shower effect", new()
                    {
                        new RuleFieldEffect("Meteor shower effect", AppSettings.Effects.FirstOrDefault()),
                        new RuleFieldWorldPointSet("Meteor shower point set", map.WorldPointSets.FirstOrDefault())
                    });
                    AddRuleFieldObservableCollection("Has rotational winds", new()
                    {
                        new RuleFieldDouble("Wind magnitude"), //might need to be >= 0
                        new RuleFieldDouble("Wind damage frequency"), //might need to be >= 0
                    });
                    AddRuleFieldBool("Nebula cloud eneergy drain");
                    AddRuleFieldBool("Nebula occlusion");
                    AddRuleFieldDouble("Ambient sound max distance");
                    break;
                case Enums.RuleAction.StateInitSetupShip:
                    RuleFields.Clear();
                    ShipUnit = new(map, NamedElement.GenerateName("Ship", map.ShipUnits));
                    map.ShipUnits.Add(ShipUnit);
                    AddRuleFieldWorldObject("World object");
                    var shipNameField = new RuleFieldString("Ship name", ShipUnit.Name);
                    shipNameField.PropertyChanged += (v, e) =>
                    {
                        if(v is RuleFieldString rfs && e.PropertyName == "Value")
                        {
                            ShipUnit.Name = rfs.Value;
                        }
                    };
                    RuleFields.Add(shipNameField);
                    AddRuleFieldPath("Ship path", isOptional: true, optionalLabel: "Has path");
                    AddRuleFieldFollowMode("Follow mode");
                    AddRuleFieldAiStance("AI stance");
                    AddRuleFieldPlayer("Player/Owner", isOptional: true, optionalLabel: "Has player/owner");
                    AddRuleFieldBool("Primary ship");
                    AddRuleFieldCrewSkillLevel("Crew skill level");
                    AddRuleFieldBool("Boardable");
                    AddRuleFieldShipName("Localized ship name");
                    break;
                case Enums.RuleAction.AddVictoryPointsForSinglePlayer:
                    RuleFields.Clear();
                    AddRuleFieldInt("Victory points to be added"); //might need to be >= 0
                    break;
                case Enums.RuleAction.BreakTow:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Tower group A");
                    AddRuleFieldGroupUnit("Tower group B"); //should probably be unit
                    break;
                case Enums.RuleAction.ClearGroupUnitBorderZone:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.ClearAllAICommands:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.CloseHUDTextureOverlay:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.CreateReleaseEventEffect:
                    RuleFields.Clear();
                    AddRuleFieldWorldPointSet("Point set");
                    AddRuleFieldEffect("Effect");
                    AddRuleFieldBool("State");
                    break;
                case Enums.RuleAction.CrewSpeechHelmOffCourse:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.CrewSpeechToggleOnOff:
                    RuleFields.Clear();
                    AddRuleFieldBool("Crew speech state");
                    break;
                case Enums.RuleAction.DamageGroupUnitByXPercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.DestroyGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.DockShips:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Docking group");
                    AddRuleFieldGroupUnit("Target to dock");
                    break;
                case Enums.RuleAction.DragonSetAIStance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dragon group");
                    AddRuleFieldAiStance("AI stance");
                    break;
                case Enums.RuleAction.DragonSetDamageThreshold:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dragon group");
                    AddRuleFieldDouble("Damage threshold", min: 0, max: 1);
                    break;
                case Enums.RuleAction.EndGame:
                    RuleFields.Clear();
                    AddRuleFieldObservableCollection("Use custom message", new()
                    {
                        new RuleFieldInGameMessage("Winner", StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault()),
                        new RuleFieldInGameMessage("Loser", StringDictionnary.InGameMessagesDictionnary.Keys.FirstOrDefault())
                    });
                    AddRuleFieldBool("Show stats screen");
                    break;
                case Enums.RuleAction.FocusCameraOnGroup:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Distance", min: 0);
                    AddRuleFieldDouble("Relative angle", min: 0, max: 360);
                    AddRuleFieldBool("Use transition");
                    break;
                case Enums.RuleAction.GotoNextLevel:
                    RuleFields.Clear();
                    AddRuleFieldSinglePlayerMission("Next level");
                    AddRuleFieldBool("Display loading string");
                    break;
                case Enums.RuleAction.GrantTeamXPoints:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldInt("Point"); //might need to be >= 0
                    break;
                case Enums.RuleAction.GroupToFollowPath:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPath("Path");
                    AddRuleFieldFollowMode("Follow mode");
                    AddRuleFieldBool("Find closest");
                    break;
                case Enums.RuleAction.GroupAToRamGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit A");
                    AddRuleFieldGroupUnit("Group/Unit B");
                    break;
                case Enums.RuleAction.GroupAToAttackGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Attack group/unit");
                    AddRuleFieldGroupUnit("Target group/unit");
                    break;
                case Enums.RuleAction.Mission9DoDarkMatterExplosion:
                    RuleFields.Clear();
                    AddRuleFieldPolygon("Affected area");
                    break;
                case Enums.RuleAction.Mission9TeleportLongboat:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Longboat group/unit");
                    break;
                case Enums.RuleAction.NISAttachCamera:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Distance", min: 0);
                    AddRuleFieldDouble("Angle XY", min: 0, max: 360);
                    AddRuleFieldDouble("Angle YZ", min: 0, max: 360);
                    break;
                case Enums.RuleAction.NISEnd:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.NISFocusCameraOnGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.NISFocusCameraOnPoint:
                    RuleFields.Clear();
                    AddRuleFieldWorldPointSet("Point set");
                    break;
                case Enums.RuleAction.NISFocusOnMainShip:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.NISPositionCameraRelativeToObject:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Distance", min: 0);
                    AddRuleFieldDouble("Angle XY", min: 0, max: 360);
                    AddRuleFieldDouble("Angle YZ", min: 0, max: 360);
                    AddRuleFieldBool("Jump to point");
                    break;
                case Enums.RuleAction.NISSetCameraPath:
                    RuleFields.Clear();
                    AddRuleFieldPath("Path");
                    AddRuleFieldBool("Jump to point");
                    break;
                case Enums.RuleAction.NISSetCameraSpeed:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Acceleration", min: 0);
                    AddRuleFieldDouble("Max velocity", min: 0);
                    break;
                case Enums.RuleAction.NISSetTransitionCameraSpeed:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Acceleration", min: 0);
                    AddRuleFieldDouble("Max velocity", min: 0);
                    break;
                case Enums.RuleAction.NISStart:
                    RuleFields.Clear();
                    AddRuleFieldBool("All objects visible");
                    AddRuleFieldBool("Open NIS bars instantly");
                    break;
                case Enums.RuleAction.NISToggleAllObjectsVisibility:
                    RuleFields.Clear();
                    AddRuleFieldBool("All objects visible");
                    break;
                case Enums.RuleAction.NISToggleNISModeGunAccuracy:
                    RuleFields.Clear();
                    AddRuleFieldBool("Use NIS gun accuracy");
                    break;
                case Enums.RuleAction.NISZoom: //to be tested (values are probably not very high)
                    RuleFields.Clear();
                    AddRuleFieldDouble("FOV");
                    AddRuleFieldDouble("Speed");
                    break;
                case Enums.RuleAction.OpenCrewAndArmsScreens:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.OpenHUDTextureOverlay:
                    RuleFields.Clear();
                    AddRuleFieldGuiTexture("Texture");
                    break;
                case Enums.RuleAction.OpenWeaponBar:
                    RuleFields.Clear();
                    AddRuleFieldBool("Open state");
                    break;
                case Enums.RuleAction.PlayMusicTrack:
                    RuleFields.Clear();
                    AddRuleFieldMusic("Track");
                    AddRuleFieldBool("Crossfade transition");
                    AddRuleFieldDouble("Fade out time (secs)", min: 0);
                    AddRuleFieldDouble("Fade in time (secs)", min: 0);
                    AddRuleFieldDouble("New volume", min: 0, max: 1);
                    break;
                case Enums.RuleAction.PlaySpecialEffect:
                    RuleFields.Clear();
                    AddRuleFieldDialogueAudio("File name");
                    AddRuleFieldBool("Play as dialogue");
                    break;
                case Enums.RuleAction.PlaySpeechEvent:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech event");
                    break;
                case Enums.RuleAction.RemainingTeamWins:
                    RuleFields.Clear();
                    break;
                case Enums.RuleAction.ResetHitCount:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.ResetShotsFiredCount:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePoint:
                    RuleFields.Clear();
                    AddRuleFieldObjectivePoint("Objective point");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointOnShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointVisibleOnStarmap:
                    RuleFields.Clear();
                    AddRuleFieldBool("Visible on starmap");
                    break;
                case Enums.RuleAction.SetDockTime:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Docking group/unit");
                    AddRuleFieldDouble("Dock time", min: 0);
                    break;
                case Enums.RuleAction.SetFlagAction:
                    RuleFields.Clear();
                    AddRuleFieldFlag("Flag name");
                    AddRuleFieldBool("Value");
                    break;
                case Enums.RuleAction.SetFleetHoldFire:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player fleet");
                    AddRuleFieldBool("Hold fire");
                    break;
                case Enums.RuleAction.SetFleetHoldFormation:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player fleet");
                    AddRuleFieldBool("Hold formation");
                    break;
                case Enums.RuleAction.SetFleetPrimaryShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("New primary ship"); //should probably be unit
                    break;
                case Enums.RuleAction.SetFleetFormationType:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player fleet");
                    AddRuleFieldFormationType("Formation type");
                    AddRuleFieldBool("Hold formation");
                    break;
                case Enums.RuleAction.SetGroupSpaceObjectsVelocity:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group"); //Should probably be a group and not unit
                    AddRuleFieldInt("Velocity", min: 0); //can probably < 0, just for fun
                    break;
                case Enums.RuleAction.SetGroupThrottlePercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Throttle percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetGroupUnitAICaptain:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldAiStance("AI stance");
                    break;
                case Enums.RuleAction.SetGroupUnitAIStance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldAiStance("AI stance");
                    break;
                case Enums.RuleAction.SetGroupUnitBoardable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Boardable");
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZone:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPolygon("Border zone");
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZoneReturnPoint:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldWorldPointSet("Return point");
                    break;
                case Enums.RuleAction.SetGroupUnitCloakState:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Cloak state");
                    AddRuleFieldBool("AI can override");
                    break;
                case Enums.RuleAction.SetGroupUnitDockable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Dockable");
                    break;
                case Enums.RuleAction.SetGroupUnitHoldPosition:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Hold position");
                    break;
                case Enums.RuleAction.SetGroupUnitMissionEssential:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Mission essential");
                    break;
                case Enums.RuleAction.SetGroupUnitMovable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Movable");
                    break;
                case Enums.RuleAction.SetGroupUnitOwner:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPlayer("New owner");
                    break;
                case Enums.RuleAction.SetGroupUnitTowable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Towable");
                    break;
                case Enums.RuleAction.SetGroupUnitVisibility:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Visible");
                    break;
                case Enums.RuleAction.SetGroupUnitVulnerability:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Invulnerable percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetGroupUnitWarningShotMode:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Warning shot");
                    break;
                case Enums.RuleAction.SetIsInCalyanAbyss:
                    RuleFields.Clear();
                    AddRuleFieldBool("Caylan abyss");
                    break;
                case Enums.RuleAction.SetLifeboatCreationState:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Lifeboat");
                    break;
                case Enums.RuleAction.SetMapTextVisibility:
                    RuleFields.Clear();
                    AddRuleFieldMapTextPoint("Map text");
                    AddRuleFieldBool("Visible on starmap");
                    break;
                case Enums.RuleAction.SetMaxThrottlePercent:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Throttle percent", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetNebulaFadeoutDistance:
                    RuleFields.Clear();
                    AddRuleFieldDouble("Fadeout distance", min: 0);
                    break;
                case Enums.RuleAction.SetNebulaToLockObjects:
                    RuleFields.Clear();
                    AddRuleFieldBool("Lock objects");
                    break;
                case Enums.RuleAction.SetObjectiveTaskActiveState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective task");
                    AddRuleFieldBool("Active state");
                    break;
                case Enums.RuleAction.SetObjectiveTaskCompleteState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective task");
                    AddRuleFieldBool("Complete state");
                    break;
                case Enums.RuleAction.SetObjectiveTaskFailedState:
                    RuleFields.Clear();
                    AddRuleFieldObjectiveTask("Objective task");
                    AddRuleFieldBool("Failed state");
                    break;
                case Enums.RuleAction.SetRadarActiveState:
                    RuleFields.Clear();
                    AddRuleFieldBool("Radar active state");
                    break;
                case Enums.RuleAction.SetShipBannerType:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBannerType("Banner type");
                    break;
                case Enums.RuleAction.SetShipName:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldShipName("New ship name");
                    break;
                case Enums.RuleAction.SetShipTopSpeedPercentage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldDouble("Throttle speed", min: 0, max: 1);
                    break;
                case Enums.RuleAction.SetShipsFlagTexture:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldFlagTexture("Flag texture");
                    break;
                case Enums.RuleAction.SetAllianceBetweenPlayerAToTRUEFALSE:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player A");
                    AddRuleFieldPlayer("Player B");
                    AddRuleFieldBool("Alliance");
                    break;
                case Enums.RuleAction.SetCollidable:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldBool("Collidable");
                    break;
                case Enums.RuleAction.SetupAsteroidBelt:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Asteroid group");
                    AddRuleFieldPath("Path");
                    AddRuleFieldFollowMode("Follow mode");
                    AddRuleFieldBool("Find closest");
                    AddRuleFieldDouble("Velocity Upper m/sec");
                    AddRuleFieldDouble("Velocity Lower m/sec");
                    AddRuleFieldDouble("Tumble Upper Rads/sec");
                    AddRuleFieldDouble("Tumble Lower Rads/sec");
                    break;
                case Enums.RuleAction.SetupSpaceAnimalFlock:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPath("Path");
                    AddRuleFieldFollowMode("Follow mode");
                    AddRuleFieldBool("Find closest");
                    AddRuleFieldInt("Velocity"); //can probably be < 0, just the fun of going reverse
                    break;
                case Enums.RuleAction.SetupTeamObjective:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldObjectivePoint("Objective point");
                    AddRuleFieldObjectiveTask("Objective task");
                    break;
                case Enums.RuleAction.StartTimer:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer");
                    break;
                case Enums.RuleAction.StopTimer:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer");
                    break;
                case Enums.RuleAction.TutorialPauseWhenStarmapOpens:
                    RuleFields.Clear();
                    AddRuleFieldBool("Pause when star map opens");
                    break;
                case Enums.RuleAction.TeamXWins:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    break;
                case Enums.RuleAction.TeleportGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldWorldPointSet("Point set");
                    break;
                case Enums.RuleAction.ToggleIslandRepairWhenDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group");
                    AddRuleFieldBool("Repair when docked");
                    break;
                case Enums.RuleAction.TowShip:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Towing group");
                    AddRuleFieldGroupUnit("Target to tow"); //might need to be unit
                    break;
                case Enums.RuleAction.TransferGroupUnitToGroup:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit to transfer");
                    AddRuleFieldGroup("Target group");
                    break;
                default:
                    break;
            }
        }
    }
}
