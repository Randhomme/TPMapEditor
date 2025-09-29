using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Enums;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class RuleAction : ObservableObject
    {
        private WorldMap map;
        [ObservableProperty]
        private Enums.RuleAction type;
        [ObservableProperty]
        private ShipUnit? shipUnit; //for ship unit creation/edition
        [ObservableProperty]
        private NamedElement? nebulaCloudPointSet, solarStormPointSet, meteorShowerPointSet, shipPath, pointSet;
        [ObservableProperty]
        private bool hasCrewSkillLevel, hasAiStance, hasLightningOnOff, lightningOnOff, hasMeteorsOnOff, meteorsOnOff, hasHasNebulaCloudEffect, hasNebulaCloudEffect, hasHasSolarStormEffect, hasSolarStormEffect, hasHasMeteorShowerEffect, hasMeteorShowerEffect, hasRotationalWindsOnOff, rotationalWindsOnOff, hasShipPath, hasFollowMode, hasLocalizedShipName, hasPointSet, hasEffect, hasUseCustomMessage, useCustomMessage;
        [ObservableProperty]
        private bool? nebulaCloudDrainEnergyOnOff, nebulaOcclusionOnOff, isPrimaryShip, isBoardable, state, crewSpeechState, showStatsScreen, useTransition; //null if not used
        [ObservableProperty]
        private string? nebulaCloudEffectName, solarStormEffectName, meteorShowerEffectName, localizedShipName, effectName, winnerCustomMessageStringId, loserCustomMessageStringId;
        [ObservableProperty]
        private int? victoryPointsToBeAdded;
        [ObservableProperty]
        private double? lightningBlastRechargeTime, meteorStrikeRechargeTime, windMagnitude, windDamageFrequency, ambientSoundMaxDistance, percent, damageThreshold, distance, relativeAngle;
        [ObservableProperty]
        private CrewSkillLevel crewSkillLevel;
        [ObservableProperty]
        private AiStance aiStance;
        [ObservableProperty]
        private FollowMode followMode;

        [ObservableProperty]
        private bool? isGroupUnit1Unit, isGroupUnit2Unit, hasOptionalPlayer1;
        [ObservableProperty]
        private bool hasWorldObject1, hasGroup1, hasPath1, hasPolygon1;
        [ObservableProperty]
        private string? worldObject1Label, groupUnit1Label, groupUnit2Label, group1Label, path1Label, optionalPlayer1Label, polygon1Label, string1Label, int1Label;
        [ObservableProperty]
        private WorldObject? worldObject1; //reference to object in case its id changes after reordering
        [ObservableProperty]
        private NamedElement? groupUnit1, groupUnit2, group1, path1, optionalPlayer1, polygon1;
        [ObservableProperty]
        private string? string1;
        [ObservableProperty]
        private int? int1;

        public RuleAction(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(Enums.RuleAction.StateInitSetupEtheriumCurrent);
        }

        private void SetDefaults()
        {
            //objects
            WorldObject1 = null;
            GroupUnit1 = GroupUnit2 = Group1 = Path1 = OptionalPlayer1 = Polygon1 = null;
            NebulaCloudPointSet = SolarStormPointSet = MeteorShowerPointSet = ShipPath = PointSet = null;
            if (ShipUnit != null)
            {
                map.ShipUnits.Remove(ShipUnit);
                ShipUnit = null;
            }

            //strings
            WorldObject1Label = GroupUnit1Label = GroupUnit2Label = Group1Label = Path1Label = OptionalPlayer1Label = Polygon1Label = String1Label = string.Empty;
            String1 = null;
            NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = LocalizedShipName = EffectName = WinnerCustomMessageStringId = LoserCustomMessageStringId = null;

            //ints
            Int1 = VictoryPointsToBeAdded = null;

            //doubles (float in the map file)
            LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = Percent = DamageThreshold = Distance = RelativeAngle = null;

            //bools
            IsGroupUnit1Unit = IsGroupUnit2Unit = HasOptionalPlayer1 = null;
            HasWorldObject1 = HasGroup1 = HasPath1 = HasPolygon1 = false;
            HasCrewSkillLevel = HasLightningOnOff = LightningOnOff = HasMeteorsOnOff = MeteorsOnOff = HasHasNebulaCloudEffect = HasNebulaCloudEffect = HasHasSolarStormEffect = HasSolarStormEffect = HasHasMeteorShowerEffect = HasMeteorShowerEffect = HasRotationalWindsOnOff = RotationalWindsOnOff = HasShipPath = HasFollowMode = HasLocalizedShipName = HasPointSet = HasEffect = HasUseCustomMessage = UseCustomMessage = false;
            NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = IsPrimaryShip = IsBoardable = State = CrewSpeechState = ShowStatsScreen = UseTransition = null;

            //enums
            CrewSkillLevel = CrewSkillLevel.CREWSKILLLEVEL;
            AiStance = AiStance.AISTANCE;
            FollowMode = FollowMode.Loop;
        }

        partial void OnTypeChanged(Enums.RuleAction value)
        {
            switch (value)
            {
                case Enums.RuleAction.StateInitSetupEtheriumCurrent:
                    SetDefaults();
                    WorldObject1Label = "World object";
                    Path1Label = "Etherium current path";
                    String1Label = "Etherium current name";
                    HasWorldObject1 = HasPath1 = true;
                    WorldObject1 = map.WorldObjects.FirstOrDefault();
                    String1 = "Etherium Current";
                    Path1 = map.WaypointPaths.FirstOrDefault();
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    SetDefaults();
                    WorldObject1Label = "World object";
                    OptionalPlayer1Label = "Player/Owner";
                    Int1Label = "Combat strength";
                    HasOptionalPlayer1 = HasWorldObject1 = HasCrewSkillLevel = HasAiStance = true;
                    WorldObject1 = map.WorldObjects.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    SetDefaults();
                    WorldObject1Label = "World object";
                    Polygon1Label = "Polygon";
                    String1Label = "Nebula name";
                    HasWorldObject1 = HasPolygon1 = HasLightningOnOff = HasMeteorsOnOff = HasNebulaCloudEffect = HasHasNebulaCloudEffect = HasHasSolarStormEffect = HasHasMeteorShowerEffect = HasRotationalWindsOnOff = true;
                    NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = LightningOnOff = MeteorsOnOff = RotationalWindsOnOff = HasSolarStormEffect = HasMeteorShowerEffect = false;
                    WorldObject1 = map.WorldObjects.FirstOrDefault();
                    String1 = "Nebula";
                    Polygon1 = map.WorldPolygons.FirstOrDefault();
                    LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = 0;
                    NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = AppSettings.Effects.FirstOrDefault();
                    NebulaCloudPointSet = SolarStormPointSet = MeteorShowerPointSet = map.WorldPoints.FirstOrDefault();
                    break;
                case Enums.RuleAction.StateInitSetupShip:
                    SetDefaults();
                    ShipUnit = new(map, NamedElement.GenerateName("Ship", map.ShipUnits));
					map.ShipUnits.Add(ShipUnit);
                    WorldObject1Label = "World object";
                    OptionalPlayer1Label = "Player/Owner";
                    HasOptionalPlayer1 = HasWorldObject1 = HasShipPath = HasFollowMode = HasAiStance = HasCrewSkillLevel = HasLocalizedShipName = true;
                    IsPrimaryShip = IsBoardable = false;
                    ShipPath = map.WaypointPaths.FirstOrDefault();
                    LocalizedShipName = ShipUnit.ShipNamesDictionnary.Keys.FirstOrDefault();
                    break;
                case Enums.RuleAction.AddVictoryPointsForSinglePlayer:
                    SetDefaults();
                    VictoryPointsToBeAdded = 0;
                    break;
                case Enums.RuleAction.BreakTow:
                    SetDefaults();
                    GroupUnit1Label = "Tower group A";
                    GroupUnit2Label = "Tower group B";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = GroupUnit2 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.ClearGroupUnitBorderZone:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.ClearAllAICommands:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.CloseHUDTextureOverlay:
                    SetDefaults();
                    break;
                case Enums.RuleAction.CreateReleaseEventEffect:
                    SetDefaults();
                    State = HasPointSet = HasEffect = true;
                    EffectName = AppSettings.Effects.FirstOrDefault();
                    PointSet = map.WorldPoints.FirstOrDefault();
                    break;
                case Enums.RuleAction.CrewSpeechHelmOffCourse:
                    SetDefaults();
                    break;
                case Enums.RuleAction.CrewSpeechToggleOnOff:
                    SetDefaults();
                    CrewSpeechState = true;
                    break;
                case Enums.RuleAction.DamageGroupUnitByXPercent:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Percent = 0;
                    break;
                case Enums.RuleAction.DestroyGroupUnit:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DockShips:
                    SetDefaults();
                    GroupUnit1Label = "Docking group/unit";
                    GroupUnit2Label = "Target to dock to";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = GroupUnit2 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DragonSetAIStance:
                    SetDefaults();
                    Group1Label = "Dragon group";
                    HasGroup1 = HasAiStance = true;
                    Group1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DragonSetDamageThreshold:
                    SetDefaults();
                    Group1Label = "Dragon group";
                    HasGroup1 = true;
                    Group1 = map.Groups.FirstOrDefault();
                    DamageThreshold = 0;
                    break;
                case Enums.RuleAction.EndGame:
                    SetDefaults();
                    ShowStatsScreen = HasUseCustomMessage = true;
                    WinnerCustomMessageStringId = LoserCustomMessageStringId = WorldMap.InGameMessages.Keys.FirstOrDefault();
                    break;
                case Enums.RuleAction.FocusCameraOnGroup:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = UseTransition = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Distance = RelativeAngle = 0;
                    break;
                case Enums.RuleAction.GotoNextLevel:
                    SetDefaults();
                    break;
                case Enums.RuleAction.GrantTeamXPoints:
                    SetDefaults();
                    break;
                case Enums.RuleAction.GroupToFollowPath:
                    SetDefaults();
                    break;
                case Enums.RuleAction.GroupAToRamGroupB:
                    SetDefaults();
                    break;
                case Enums.RuleAction.GroupAToAttackGroupB:
                    SetDefaults();
                    break;
                case Enums.RuleAction.Mission9DoDarkMatterExplosion:
                    SetDefaults();
                    break;
                case Enums.RuleAction.Mission9TeleportLongboat:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISAttachCamera:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISEnd:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISFocusCameraOnGroupUnit:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISFocusCameraOnPoint:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISFocusOnMainShip:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISPositionCameraRelativeToObject:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISSetCameraPath:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISSetCameraSpeed:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISSetTransitionCameraSpeed:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISStart:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISToggleAllObjectsVisibility:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISToggleNISModeGunAccuracy:
                    SetDefaults();
                    break;
                case Enums.RuleAction.NISZoom:
                    SetDefaults();
                    break;
                case Enums.RuleAction.OpenCrewAndArmsScreens:
                    SetDefaults();
                    break;
                case Enums.RuleAction.OpenHUDTextureOverlay:
                    SetDefaults();
                    break;
                case Enums.RuleAction.OpenWeaponBar:
                    SetDefaults();
                    break;
                case Enums.RuleAction.PlayMusicTrack:
                    SetDefaults();
                    break;
                case Enums.RuleAction.PlaySpecialEffect:
                    SetDefaults();
                    break;
                case Enums.RuleAction.PlaySpeechEvent:
                    SetDefaults();
                    break;
                case Enums.RuleAction.RemainingTeamWins:
                    SetDefaults();
                    break;
                case Enums.RuleAction.ResetHitCount:
                    SetDefaults();
                    break;
                case Enums.RuleAction.ResetShotsFiredCount:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetCurrentObjectivePoint:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointOnShip:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointVisibleOnStarmap:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetDockTime:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetFlagAction:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetFleetHoldFire:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetFleetHoldFormation:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetFleetPrimaryShip:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetFleetFormationType:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupSpaceObjectsVelocity:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupThrottlePercent:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitAICaptain:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitAIStance:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitBoardable:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZone:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZoneReturnPoint:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitCloakState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitDockable:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitHoldPosition:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitMissionEssential:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitMovable:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitOwner:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitTowable:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitVisibility:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitVulnerability:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetGroupUnitWarningShotMode:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetIsInCalyanAbyss:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetLifeboatCreationState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetMapTextVisibility:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetMaxThrottlePercent:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetNebulaFadeoutDistance:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetNebulaToLockObjects:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetObjectiveTaskActiveState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetObjectiveTaskCompleteState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetObjectiveTaskFailedState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetRadarActiveState:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetShipBannerType:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetShipName:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetShipTopSpeedPercentage:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetShipsFlagTexture:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetAllianceBetweenPlayerAToTRUEFALSE:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetCollidable:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetupAsteroidBelt:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetupSpaceAnimalFlock:
                    SetDefaults();
                    break;
                case Enums.RuleAction.SetupTeamObjective:
                    SetDefaults();
                    break;
                case Enums.RuleAction.StartTimer:
                    SetDefaults();
                    break;
                case Enums.RuleAction.StopTimer:
                    SetDefaults();
                    break;
                case Enums.RuleAction.TutorialPauseWhenStarmapOpens:
                    SetDefaults();
                    break;
                case Enums.RuleAction.TeamXWins:
                    SetDefaults();
                    break;
                case Enums.RuleAction.TeleportGroupUnit:
                    SetDefaults();
                    break;
                case Enums.RuleAction.ToggleIslandRepairWhenDocked:
                    SetDefaults();
                    break;
                case Enums.RuleAction.TowShip:
                    SetDefaults();
                    break;
                default:
                    break;
            }
        }
    }
}
