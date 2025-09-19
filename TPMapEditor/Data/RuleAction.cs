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
        private WorldObject? worldObject; //reference to object in case its id changesa after reordering
        [ObservableProperty]
        private ShipUnit? shipUnit; //for ship unit creation/edition
        [ObservableProperty]
        private NamedElement? towerGroupA, towerGroupB, groupUnit, dockingGroup, targetToDockTo; //group/unit
        [ObservableProperty]
        private NamedElement? group; //group
        [ObservableProperty]
        private NamedElement? etheriumPath, playerOwner, polygon, nebulaCloudPointSet, solarStormPointSet, meteorShowerPointSet, shipPath, pointSet;
        [ObservableProperty]
        private bool hasWorldObject, hasEtheriumPath, hasHasPlayerOwner, hasPlayerOwner, hasCrewSkillLevel, hasAiStance, hasPolygon, hasLightningOnOff, lightningOnOff, hasMeteorsOnOff, meteorsOnOff, hasHasNebulaCloudEffect, hasNebulaCloudEffect, hasHasSolarStormEffect, hasSolarStormEffect, hasHasMeteorShowerEffect, hasMeteorShowerEffect, hasRotationalWindsOnOff, rotationalWindsOnOff, hasShipPath, hasFollowMode, hasLocalizedShipName, hasPointSet, hasEffect, hasGroup, hasUseCustomMessage, useCustomMessage;
        [ObservableProperty]
        private bool? nebulaCloudDrainEnergyOnOff, nebulaOcclusionOnOff, isPrimaryShip, isBoardable, isTowerGroupAUnit, isTowerGroupBUnit, isGroupUnitUnit, state, crewSpeechState, isDockingGroupUnit, isTargetToDockToUnit, showStatsScreen, useTransition; //null if not used
        [ObservableProperty]
        private string? etheriumName, nebulaName, nebulaCloudEffectName, solarStormEffectName, meteorShowerEffectName, localizedShipName, effectName, winnerCustomMessageStringId, loserCustomMessageStringId;
        [ObservableProperty]
        private int? combatStrength, victoryPointsToBeAdded;
        [ObservableProperty]
        private double? lightningBlastRechargeTime, meteorStrikeRechargeTime, windMagnitude, windDamageFrequency, ambientSoundMaxDistance, percent, damageThreshold, distance, relativeAngle;
        [ObservableProperty]
        private CrewSkillLevel crewSkillLevel;
        [ObservableProperty]
        private AiStance aiStance;
        [ObservableProperty]
        private FollowMode followMode;

        public RuleAction(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(Enums.RuleAction.StateInitSetupEtheriumCurrent);
        }

        private void SetDefaults()
        {
            //objects
            WorldObject = null;
            EtheriumPath = PlayerOwner = Polygon = NebulaCloudPointSet = SolarStormPointSet = MeteorShowerPointSet = ShipPath = TowerGroupA = TowerGroupB = GroupUnit = PointSet = Group = null;
            if (ShipUnit != null)
            {
                map.ShipUnits.Remove(ShipUnit);
                ShipUnit = null;
            }

            //strings
            EtheriumName = NebulaName = NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = LocalizedShipName = EffectName = WinnerCustomMessageStringId = LoserCustomMessageStringId = null;

            //ints
            CombatStrength = VictoryPointsToBeAdded = null;

            //doubles (float in the map file)
            LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = Percent = DamageThreshold = Distance = RelativeAngle = null;

            //bools
            HasWorldObject = HasEtheriumPath = HasHasPlayerOwner = HasPlayerOwner = HasCrewSkillLevel = HasPolygon = HasLightningOnOff = LightningOnOff = HasMeteorsOnOff = MeteorsOnOff = HasHasNebulaCloudEffect = HasNebulaCloudEffect = HasHasSolarStormEffect = HasSolarStormEffect = HasHasMeteorShowerEffect = HasMeteorShowerEffect = HasRotationalWindsOnOff = RotationalWindsOnOff = HasShipPath = HasFollowMode = HasLocalizedShipName = HasPointSet = HasEffect = HasGroup = HasUseCustomMessage = UseCustomMessage = false;
            NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = IsPrimaryShip = IsBoardable = IsTowerGroupAUnit = IsTowerGroupBUnit = IsGroupUnitUnit = State = CrewSpeechState = IsDockingGroupUnit = IsTargetToDockToUnit = ShowStatsScreen = UseTransition = null;

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
                    HasWorldObject = HasEtheriumPath = true;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    EtheriumName = "Etherium Current";
                    EtheriumPath = map.WaypointPaths.FirstOrDefault();
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    SetDefaults();
                    HasWorldObject = HasHasPlayerOwner = HasCrewSkillLevel = HasAiStance = true;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    CombatStrength = 0;
                    HasPlayerOwner = false;
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    SetDefaults();
                    HasWorldObject = HasPolygon = HasLightningOnOff = HasMeteorsOnOff = HasNebulaCloudEffect = HasHasNebulaCloudEffect = HasHasSolarStormEffect = HasHasMeteorShowerEffect = HasRotationalWindsOnOff = true;
                    NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = LightningOnOff = MeteorsOnOff = RotationalWindsOnOff = HasSolarStormEffect = HasMeteorShowerEffect = false;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    NebulaName = "Nebula";
                    Polygon = map.WorldPolygons.FirstOrDefault();
                    LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = 0;
                    NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = AppSettings.Effects.FirstOrDefault();
                    NebulaCloudPointSet = SolarStormPointSet = MeteorShowerPointSet = map.WorldPoints.FirstOrDefault();
                    break;
                case Enums.RuleAction.StateInitSetupShip:
                    SetDefaults();
                    ShipUnit = new(map, NamedElement.GenerateName("Ship", map.ShipUnits));
					map.ShipUnits.Add(ShipUnit);
                    HasWorldObject = HasShipPath = HasFollowMode = HasAiStance = HasHasPlayerOwner = HasCrewSkillLevel = HasLocalizedShipName = true;
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
                    IsTowerGroupAUnit = IsTowerGroupBUnit = false;
                    TowerGroupA = TowerGroupB = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.ClearGroupUnitBorderZone:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.ClearAllAICommands:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
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
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    Percent = 0;
                    break;
                case Enums.RuleAction.DestroyGroupUnit:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DockShips:
                    SetDefaults();
                    IsDockingGroupUnit = IsTargetToDockToUnit = false;
                    DockingGroup = TargetToDockTo = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DragonSetAIStance:
                    SetDefaults();
                    HasGroup = HasAiStance = true;
                    Group = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleAction.DragonSetDamageThreshold:
                    SetDefaults();
                    HasGroup = true;
                    Group = map.Groups.FirstOrDefault();
                    DamageThreshold = 0;
                    break;
                case Enums.RuleAction.EndGame:
                    SetDefaults();
                    ShowStatsScreen = HasUseCustomMessage = true;
                    WinnerCustomMessageStringId = LoserCustomMessageStringId = WorldMap.InGameMessages.Keys.FirstOrDefault();
                    break;
                case Enums.RuleAction.FocusCameraOnGroup:
                    SetDefaults();
                    IsGroupUnitUnit = UseTransition = false;
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
