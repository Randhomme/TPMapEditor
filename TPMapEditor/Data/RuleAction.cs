using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class RuleAction : ObservableObject
    {
        [ObservableProperty]
        private Enums.RuleAction type;

        partial void OnTypeChanged(Enums.RuleAction value)
        {
            switch (value)
            {
                case Enums.RuleAction.StateInitSetupEtheriumCurrent:
                    break;
                case Enums.RuleAction.StateInitSetupIsland:
                    break;
                case Enums.RuleAction.StateInitSetupNebula:
                    break;
                case Enums.RuleAction.StateInitSetupShip:
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
