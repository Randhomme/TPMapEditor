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
        private ShipUnit? shipUnit;
        [ObservableProperty]
        private bool hasWorldObject, hasEtheriumPathName, hasHasPlayerOwnerName, hasPlayerOwnerName, hasCrewSkillLevel, hasAiStance, hasPolygonName, hasLightningOnOff, lightningOnOff, hasMeteorsOnOff, meteorsOnOff, hasHasNebulaCloudEffect, hasNebulaCloudEffect, hasHasSolarStormEffect, hasSolarStormEffect, hasHasMeteorShowerEffect, hasMeteorShowerEffect, hasRotationalWindsOnOff, rotationalWindsOnOff, hasShipPathName, hasFollowMode, hasLocalizedShipName;
        [ObservableProperty]
        private bool? nebulaCloudDrainEnergyOnOff, nebulaOcclusionOnOff, isPrimaryShip, isBoardable; //null if not used, true/false for on/off
        [ObservableProperty]
        private string? etheriumName, etheriumPathName, playerOwnerName, nebulaName, polygonName, nebulaCloudEffectName, solarStormEffectName, meteorShowerEffectName, nebulaCloudPointSetName, solarStormPointSetName, meteorShowerPointSetName, shipPathName, localizedShipName;
        [ObservableProperty]
        private int? combatStrength;
        [ObservableProperty]
        private double? lightningBlastRechargeTime, meteorStrikeRechargeTime, windMagnitude, windDamageFrequency, ambientSoundMaxDistance;
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
            if (ShipUnit != null)
            {
                map.ShipUnits.Remove(ShipUnit);
                ShipUnit = null;
            }

            //strings
            EtheriumName = EtheriumPathName = PlayerOwnerName = NebulaName = PolygonName = NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = NebulaCloudPointSetName = SolarStormPointSetName = MeteorShowerPointSetName = ShipPathName = LocalizedShipName = null;

            //ints
            CombatStrength = null;

            //doubles (float in the map file)
            LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = null;

            //bools
            HasWorldObject = HasEtheriumPathName = HasHasPlayerOwnerName = HasPlayerOwnerName = HasCrewSkillLevel = HasPolygonName = HasLightningOnOff = LightningOnOff = HasMeteorsOnOff = MeteorsOnOff = HasNebulaCloudEffect = HasSolarStormEffect = HasMeteorShowerEffect = HasRotationalWindsOnOff = RotationalWindsOnOff = HasShipPathName = HasLocalizedShipName = false;
            NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = IsPrimaryShip = IsBoardable = null;

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
                    HasWorldObject = HasEtheriumPathName = true;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    EtheriumName = "Etherium Current";
                    EtheriumPathName = map.WaypointPaths.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    SetDefaults();
                    HasWorldObject = HasHasPlayerOwnerName = HasCrewSkillLevel = HasAiStance = true;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    CombatStrength = 0;
                    HasPlayerOwnerName = false;
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    SetDefaults();
                    HasWorldObject = HasPolygonName = HasLightningOnOff = HasMeteorsOnOff = HasNebulaCloudEffect = HasHasNebulaCloudEffect = HasHasSolarStormEffect = HasHasMeteorShowerEffect = HasRotationalWindsOnOff = true;
                    NebulaCloudDrainEnergyOnOff = NebulaOcclusionOnOff = LightningOnOff = MeteorsOnOff = RotationalWindsOnOff = HasSolarStormEffect = HasMeteorShowerEffect = false;
                    WorldObject = map.WorldObjects.FirstOrDefault();
                    NebulaName = "Nebula";
                    PolygonName = map.WorldPolygons.FirstOrDefault()?.Name;
                    LightningBlastRechargeTime = MeteorStrikeRechargeTime = WindMagnitude = WindDamageFrequency = AmbientSoundMaxDistance = 0;
                    NebulaCloudEffectName = SolarStormEffectName = MeteorShowerEffectName = AppSettings.Effects.FirstOrDefault();
                    NebulaCloudPointSetName = SolarStormPointSetName = MeteorShowerPointSetName = map.WorldPoints.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleAction.StateInitSetupShip:
                    SetDefaults();
                    ShipUnit = new(map, NamedElement.GenerateName("Ship", map.ShipUnits));
					map.ShipUnits.Add(ShipUnit);
                    HasWorldObject = HasShipPathName = HasFollowMode = HasAiStance = HasHasPlayerOwnerName = HasCrewSkillLevel = HasLocalizedShipName = true;
                    IsPrimaryShip = IsBoardable = false;
                    ShipPathName = map.WaypointPaths.FirstOrDefault()?.Name;
                    LocalizedShipName = ShipUnit.ShipNamesDictionnary.Keys.FirstOrDefault();
                    break;
                case Enums.RuleAction.AddVictoryPointsForSinglePlayer:
                    break;
                case Enums.RuleAction.BreakTow:
                    break;
                case Enums.RuleAction.ClearGroupUnitBorderZone:
                    break;
                case Enums.RuleAction.ClearAllAICommands:
                    break;
                case Enums.RuleAction.CloseHUDTextureOverlay:
                    break;
                case Enums.RuleAction.CreateReleaseEventEffect:
                    break;
                case Enums.RuleAction.CrewSpeechHelmOffCourse:
                    break;
                case Enums.RuleAction.CrewSpeechToggleOnOff:
                    break;
                case Enums.RuleAction.DamageGroupUnitByXPercent:
                    break;
                case Enums.RuleAction.DestroyGroupUnit:
                    break;
                case Enums.RuleAction.DockShips:
                    break;
                case Enums.RuleAction.DragonSetAIStance:
                    break;
                case Enums.RuleAction.DragonSetDamageThreshold:
                    break;
                case Enums.RuleAction.EndGame:
                    break;
                case Enums.RuleAction.FocusCameraOnGroup:
                    break;
                case Enums.RuleAction.GotoNextLevel:
                    break;
                case Enums.RuleAction.GrantTeamXPoints:
                    break;
                case Enums.RuleAction.GroupToFollowPath:
                    break;
                case Enums.RuleAction.GroupAToRamGroupB:
                    break;
                case Enums.RuleAction.GroupAToAttackGroupB:
                    break;
                case Enums.RuleAction.Mission9DoDarkMatterExplosion:
                    break;
                case Enums.RuleAction.Mission9TeleportLongboat:
                    break;
                case Enums.RuleAction.NISAttachCamera:
                    break;
                case Enums.RuleAction.NISEnd:
                    break;
                case Enums.RuleAction.NISFocusCameraOnGroupUnit:
                    break;
                case Enums.RuleAction.NISFocusCameraOnPoint:
                    break;
                case Enums.RuleAction.NISFocusOnMainShip:
                    break;
                case Enums.RuleAction.NISPositionCameraRelativeToObject:
                    break;
                case Enums.RuleAction.NISSetCameraPath:
                    break;
                case Enums.RuleAction.NISSetCameraSpeed:
                    break;
                case Enums.RuleAction.NISSetTransitionCameraSpeed:
                    break;
                case Enums.RuleAction.NISStart:
                    break;
                case Enums.RuleAction.NISToggleAllObjectsVisibility:
                    break;
                case Enums.RuleAction.NISToggleNISModeGunAccuracy:
                    break;
                case Enums.RuleAction.NISZoom:
                    break;
                case Enums.RuleAction.OpenCrewAndArmsScreens:
                    break;
                case Enums.RuleAction.OpenHUDTextureOverlay:
                    break;
                case Enums.RuleAction.OpenWeaponBar:
                    break;
                case Enums.RuleAction.PlayMusicTrack:
                    break;
                case Enums.RuleAction.PlaySpecialEffect:
                    break;
                case Enums.RuleAction.PlaySpeechEvent:
                    break;
                case Enums.RuleAction.RemainingTeamWins:
                    break;
                case Enums.RuleAction.ResetHitCount:
                    break;
                case Enums.RuleAction.ResetShotsFiredCount:
                    break;
                case Enums.RuleAction.SetCurrentObjectivePoint:
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointOnShip:
                    break;
                case Enums.RuleAction.SetCurrentObjectivePointVisibleOnStarmap:
                    break;
                case Enums.RuleAction.SetDockTime:
                    break;
                case Enums.RuleAction.SetFlagAction:
                    break;
                case Enums.RuleAction.SetFleetHoldFire:
                    break;
                case Enums.RuleAction.SetFleetHoldFormation:
                    break;
                case Enums.RuleAction.SetFleetPrimaryShip:
                    break;
                case Enums.RuleAction.SetFleetFormationType:
                    break;
                case Enums.RuleAction.SetGroupSpaceObjectsVelocity:
                    break;
                case Enums.RuleAction.SetGroupThrottlePercent:
                    break;
                case Enums.RuleAction.SetGroupUnitAICaptain:
                    break;
                case Enums.RuleAction.SetGroupUnitAIStance:
                    break;
                case Enums.RuleAction.SetGroupUnitBoardable:
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZone:
                    break;
                case Enums.RuleAction.SetGroupUnitBorderZoneReturnPoint:
                    break;
                case Enums.RuleAction.SetGroupUnitCloakState:
                    break;
                case Enums.RuleAction.SetGroupUnitDockable:
                    break;
                case Enums.RuleAction.SetGroupUnitHoldPosition:
                    break;
                case Enums.RuleAction.SetGroupUnitMissionEssential:
                    break;
                case Enums.RuleAction.SetGroupUnitMovable:
                    break;
                case Enums.RuleAction.SetGroupUnitOwner:
                    break;
                case Enums.RuleAction.SetGroupUnitTowable:
                    break;
                case Enums.RuleAction.SetGroupUnitVisibility:
                    break;
                case Enums.RuleAction.SetGroupUnitVulnerability:
                    break;
                case Enums.RuleAction.SetGroupUnitWarningShotMode:
                    break;
                case Enums.RuleAction.SetIsInCalyanAbyss:
                    break;
                case Enums.RuleAction.SetLifeboatCreationState:
                    break;
                case Enums.RuleAction.SetMapTextVisibility:
                    break;
                case Enums.RuleAction.SetMaxThrottlePercent:
                    break;
                case Enums.RuleAction.SetNebulaFadeoutDistance:
                    break;
                case Enums.RuleAction.SetNebulaToLockObjects:
                    break;
                case Enums.RuleAction.SetObjectiveTaskActiveState:
                    break;
                case Enums.RuleAction.SetObjectiveTaskCompleteState:
                    break;
                case Enums.RuleAction.SetObjectiveTaskFailedState:
                    break;
                case Enums.RuleAction.SetRadarActiveState:
                    break;
                case Enums.RuleAction.SetShipBannerType:
                    break;
                case Enums.RuleAction.SetShipName:
                    break;
                case Enums.RuleAction.SetShipTopSpeedPercentage:
                    break;
                case Enums.RuleAction.SetShipsFlagTexture:
                    break;
                case Enums.RuleAction.SetAllianceBetweenPlayerAToTRUEFALSE:
                    break;
                case Enums.RuleAction.SetCollidable:
                    break;
                case Enums.RuleAction.SetupAsteroidBelt:
                    break;
                case Enums.RuleAction.SetupSpaceAnimalFlock:
                    break;
                case Enums.RuleAction.SetupTeamObjective:
                    break;
                case Enums.RuleAction.StartTimer:
                    break;
                case Enums.RuleAction.StopTimer:
                    break;
                case Enums.RuleAction.TutorialPauseWhenStarmapOpens:
                    break;
                case Enums.RuleAction.TeamXWins:
                    break;
                case Enums.RuleAction.TeleportGroupUnit:
                    break;
                case Enums.RuleAction.ToggleIslandRepairWhenDocked:
                    break;
                case Enums.RuleAction.TowShip:
                    break;
                default:
                    break;
            }
        }
    }
}
