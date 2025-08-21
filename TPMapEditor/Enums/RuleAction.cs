using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    public enum RuleAction
    {
        [Description("*State Init* Setup Etherium Current")]
        StateInitSetupEtheriumCurrent,

        [Description("*State Init* Setup Island")]
        StateInitSetupIsland,

        [Description("*State Init* Setup Nebula")]
        StateInitSetupNebula,

        [Description("*State Init* Setup Ship")]
        StateInitSetupShip,

        [Description("Add Victory Points for SinglePlayer")]
        AddVictoryPointsForSinglePlayer,

        [Description("Break Tow")]
        BreakTow,

        [Description("Clear Group/Unit Border Zone")]
        ClearGroupUnitBorderZone,

        [Description("Clear all AI commands")]
        ClearAllAICommands,

        [Description("Close HUD Texture Overlay")]
        CloseHUDTextureOverlay,

        [Description("Create/Release Event Effect")]
        CreateReleaseEventEffect,

        [Description("Crew Speech - Helm Off Course")]
        CrewSpeechHelmOffCourse,

        [Description("Crew Speech - Toggle On Off")]
        CrewSpeechToggleOnOff,

        [Description("Damage Group/Unit by X percent")]
        DamageGroupUnitByXPercent,

        [Description("Destroy Group/Unit")]
        DestroyGroupUnit,

        [Description("Dock Ships")]
        DockShips,

        [Description("Dragon - Set AI Stance")]
        DragonSetAIStance,

        [Description("Dragon - Set Damage Threshold")]
        DragonSetDamageThreshold,

        [Description("End Game")]
        EndGame,

        [Description("Focus Camera On Group")]
        FocusCameraOnGroup,

        [Description("Goto Next Level")]
        GotoNextLevel,

        [Description("Grant Team X Points")]
        GrantTeamXPoints,

        [Description("Group to follow path")]
        GroupToFollowPath,

        [Description("GroupA to Ram GroupB")]
        GroupAToRamGroupB,

        [Description("GroupA to attack GroupB")]
        GroupAToAttackGroupB,

        [Description("Mission 9 - Do Dark Matter Explosion")]
        Mission9DoDarkMatterExplosion,

        [Description("Mission 9 - Teleport Longboat")]
        Mission9TeleportLongboat,

        [Description("NIS Attach Camera")]
        NISAttachCamera,

        [Description("NIS End")]
        NISEnd,

        [Description("NIS Focus camera on Group/Unit")]
        NISFocusCameraOnGroupUnit,

        [Description("NIS Focus camera on Point")]
        NISFocusCameraOnPoint,

        [Description("NIS Focus on Main Ship")]
        NISFocusOnMainShip,

        [Description("NIS Position Camera Relative to Object")]
        NISPositionCameraRelativeToObject,

        [Description("NIS Set Camera Path")]
        NISSetCameraPath,

        [Description("NIS Set Camera Speed")]
        NISSetCameraSpeed,

        [Description("NIS Set Transition Camera Speed")]
        NISSetTransitionCameraSpeed,

        [Description("NIS Start")]
        NISStart,

        [Description("NIS Toggle All Objects Visibility")]
        NISToggleAllObjectsVisibility,

        [Description("NIS Toggle NIS mode Gun Accuracy")]
        NISToggleNISModeGunAccuracy,

        [Description("NIS Zoom")]
        NISZoom,

        [Description("Open Crew and Arms Screens")]
        OpenCrewAndArmsScreens,

        [Description("Open HUD Texture Overlay")]
        OpenHUDTextureOverlay,

        [Description("Open Weapon Bar")]
        OpenWeaponBar,

        [Description("Play Music Track")]
        PlayMusicTrack,

        [Description("Play Special Effect")]
        PlaySpecialEffect,

        [Description("Play speech event")]
        PlaySpeechEvent,

        [Description("Remaining Team Wins")]
        RemainingTeamWins,

        [Description("Reset Hit Count")]
        ResetHitCount,

        [Description("Reset Shots Fired Count")]
        ResetShotsFiredCount,

        [Description("Set Current Objective Point")]
        SetCurrentObjectivePoint,

        [Description("Set Current Objective Point On Ship")]
        SetCurrentObjectivePointOnShip,

        [Description("Set Current Objective Point Visible On Starmap")]
        SetCurrentObjectivePointVisibleOnStarmap,

        [Description("Set Dock Time")]
        SetDockTime,

        [Description("Set Flag Action")]
        SetFlagAction,

        [Description("Set Fleet Hold Fire")]
        SetFleetHoldFire,

        [Description("Set Fleet Hold Formation")]
        SetFleetHoldFormation,

        [Description("Set Fleet Primary Ship")]
        SetFleetPrimaryShip,

        [Description("Set FleetFormation Type")]
        SetFleetFormationType,

        [Description("Set Group Space Objects velocity")]
        SetGroupSpaceObjectsVelocity,

        [Description("Set Group Throttle Percent")]
        SetGroupThrottlePercent,

        [Description("Set Group/Unit AI Captain")]
        SetGroupUnitAICaptain,

        [Description("Set Group/Unit AI Stance")]
        SetGroupUnitAIStance,

        [Description("Set Group/Unit Boardable")]
        SetGroupUnitBoardable,

        [Description("Set Group/Unit Border Zone")]
        SetGroupUnitBorderZone,

        [Description("Set Group/Unit Border Zone Return Point")]
        SetGroupUnitBorderZoneReturnPoint,

        [Description("Set Group/Unit Cloak State")]
        SetGroupUnitCloakState,

        [Description("Set Group/Unit Dockable")]
        SetGroupUnitDockable,

        [Description("Set Group/Unit Hold Position")]
        SetGroupUnitHoldPosition,

        [Description("Set Group/Unit Mission Essential")]
        SetGroupUnitMissionEssential,

        [Description("Set Group/Unit Movable")]
        SetGroupUnitMovable,

        [Description("Set Group/Unit Owner")]
        SetGroupUnitOwner,

        [Description("Set Group/Unit Towable")]
        SetGroupUnitTowable,

        [Description("Set Group/Unit Visibility")]
        SetGroupUnitVisibility,

        [Description("Set Group/Unit Vulnerability")]
        SetGroupUnitVulnerability,

        [Description("Set Group/Unit Warning Shot Mode")]
        SetGroupUnitWarningShotMode,

        [Description("Set Is In Calyan Abyss")]
        SetIsInCalyanAbyss,

        [Description("Set Lifeboat Creation State")]
        SetLifeboatCreationState,

        [Description("Set Map Text Visibility")]
        SetMapTextVisibility,

        [Description("Set Max Throttle Percent (Max user settable)")]
        SetMaxThrottlePercent,

        [Description("Set Nebula Fadeout Distance")]
        SetNebulaFadeoutDistance,

        [Description("Set Nebula to Lock Objects")]
        SetNebulaToLockObjects,

        [Description("Set Objective Task Active State")]
        SetObjectiveTaskActiveState,

        [Description("Set Objective Task Complete State")]
        SetObjectiveTaskCompleteState,

        [Description("Set Objective Task Failed State")]
        SetObjectiveTaskFailedState,

        [Description("Set Radar Active State")]
        SetRadarActiveState,

        [Description("Set Ship Banner Type")]
        SetShipBannerType,

        [Description("Set Ship Name")]
        SetShipName,

        [Description("Set Ship Top Speed Percentage")]
        SetShipTopSpeedPercentage,

        [Description("Set Ships Flag Texture")]
        SetShipsFlagTexture,

        [Description("Set alliance between PlayerA and PlayerB to TRUE/FALSE")]
        SetAllianceBetweenPlayerAToTRUEFALSE,

        [Description("SetCollidable")]
        SetCollidable,

        [Description("Setup Asteroid Belt")]
        SetupAsteroidBelt,

        [Description("Setup Space Animal Flock")]
        SetupSpaceAnimalFlock,

        [Description("Setup Team Objective")]
        SetupTeamObjective,

        [Description("Start Timer")]
        StartTimer,

        [Description("Stop Timer")]
        StopTimer,

        [Description("TUTORIAL - Pause when starmap opens?")]
        TutorialPauseWhenStarmapOpens,

        [Description("Team X Wins")]
        TeamXWins,

        [Description("Teleport Group/Unit")]
        TeleportGroupUnit,

        [Description("Toggle Island Repair When Docked")]
        ToggleIslandRepairWhenDocked,

        [Description("Tow Ship")]
        TowShip,

        [Description("Transfer Group/Unit to Group")]
        TransferGroupUnitToGroup
    }
}
