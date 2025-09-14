using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public enum RuleAction
    {
        [Display(Name = "*State Init* Setup Etherium Current")]
        StateInitSetupEtheriumCurrent,

        [Display(Name = "*State Init* Setup Island")]
        StateInitSetupIsland,

        [Display(Name = "*State Init* Setup Nebula")]
        StateInitSetupNebula,

        [Display(Name = "*State Init* Setup Ship")]
        StateInitSetupShip,

        [Display(Name = "Add Victory Points for SinglePlayer")]
        AddVictoryPointsForSinglePlayer,

        [Display(Name = "Break Tow")]
        BreakTow,

        [Display(Name = "Clear Group/Unit Border Zone")]
        ClearGroupUnitBorderZone,

        [Display(Name = "Clear all AI commands")]
        ClearAllAICommands,

        [Display(Name = "Close HUD Texture Overlay")]
        CloseHUDTextureOverlay,

        [Display(Name = "Create/Release Event Effect")]
        CreateReleaseEventEffect,

        [Display(Name = "Crew Speech - Helm Off Course")]
        CrewSpeechHelmOffCourse,

        [Display(Name = "Crew Speech - Toggle On Off")]
        CrewSpeechToggleOnOff,

        [Display(Name = "Damage Group/Unit by X percent")]
        DamageGroupUnitByXPercent,

        [Display(Name = "Destroy Group/Unit")]
        DestroyGroupUnit,

        [Display(Name = "Dock Ships")]
        DockShips,

        [Display(Name = "Dragon - Set AI Stance")]
        DragonSetAIStance,

        [Display(Name = "Dragon - Set Damage Threshold")]
        DragonSetDamageThreshold,

        [Display(Name = "End Game")]
        EndGame,

        [Display(Name = "Focus Camera On Group")]
        FocusCameraOnGroup,

        [Display(Name = "Goto Next Level")]
        GotoNextLevel,

        [Display(Name = "Grant Team X Points")]
        GrantTeamXPoints,

        [Display(Name = "Group to follow path")]
        GroupToFollowPath,

        [Display(Name = "GroupA to Ram GroupB")]
        GroupAToRamGroupB,

        [Display(Name = "GroupA to attack GroupB")]
        GroupAToAttackGroupB,

        [Display(Name = "Mission 9 - Do Dark Matter Explosion")]
        Mission9DoDarkMatterExplosion,

        [Display(Name = "Mission 9 - Teleport Longboat")]
        Mission9TeleportLongboat,

        [Display(Name = "NIS Attach Camera")]
        NISAttachCamera,

        [Display(Name = "NIS End")]
        NISEnd,

        [Display(Name = "NIS Focus camera on Group/Unit")]
        NISFocusCameraOnGroupUnit,

        [Display(Name = "NIS Focus camera on Point")]
        NISFocusCameraOnPoint,

        [Display(Name = "NIS Focus on Main Ship")]
        NISFocusOnMainShip,

        [Display(Name = "NIS Position Camera Relative to Object")]
        NISPositionCameraRelativeToObject,

        [Display(Name = "NIS Set Camera Path")]
        NISSetCameraPath,

        [Display(Name = "NIS Set Camera Speed")]
        NISSetCameraSpeed,

        [Display(Name = "NIS Set Transition Camera Speed")]
        NISSetTransitionCameraSpeed,

        [Display(Name = "NIS Start")]
        NISStart,

        [Display(Name = "NIS Toggle All Objects Visibility")]
        NISToggleAllObjectsVisibility,

        [Display(Name = "NIS Toggle NIS mode Gun Accuracy")]
        NISToggleNISModeGunAccuracy,

        [Display(Name = "NIS Zoom")]
        NISZoom,

        [Display(Name = "Open Crew and Arms Screens")]
        OpenCrewAndArmsScreens,

        [Display(Name = "Open HUD Texture Overlay")]
        OpenHUDTextureOverlay,

        [Display(Name = "Open Weapon Bar")]
        OpenWeaponBar,

        [Display(Name = "Play Music Track")]
        PlayMusicTrack,

        [Display(Name = "Play Special Effect")]
        PlaySpecialEffect,

        [Display(Name = "Play speech event")]
        PlaySpeechEvent,

        [Display(Name = "Remaining Team Wins")]
        RemainingTeamWins,

        [Display(Name = "Reset Hit Count")]
        ResetHitCount,

        [Display(Name = "Reset Shots Fired Count")]
        ResetShotsFiredCount,

        [Display(Name = "Set Current Objective Point")]
        SetCurrentObjectivePoint,

        [Display(Name = "Set Current Objective Point On Ship")]
        SetCurrentObjectivePointOnShip,

        [Display(Name = "Set Current Objective Point Visible On Starmap")]
        SetCurrentObjectivePointVisibleOnStarmap,

        [Display(Name = "Set Dock Time")]
        SetDockTime,

        [Display(Name = "Set Flag Action")]
        SetFlagAction,

        [Display(Name = "Set Fleet Hold Fire")]
        SetFleetHoldFire,

        [Display(Name = "Set Fleet Hold Formation")]
        SetFleetHoldFormation,

        [Display(Name = "Set Fleet Primary Ship")]
        SetFleetPrimaryShip,

        [Display(Name = "Set FleetFormation Type")]
        SetFleetFormationType,

        [Display(Name = "Set Group Space Objects velocity")]
        SetGroupSpaceObjectsVelocity,

        [Display(Name = "Set Group Throttle Percent")]
        SetGroupThrottlePercent,

        [Display(Name = "Set Group/Unit AI Captain")]
        SetGroupUnitAICaptain,

        [Display(Name = "Set Group/Unit AI Stance")]
        SetGroupUnitAIStance,

        [Display(Name = "Set Group/Unit Boardable")]
        SetGroupUnitBoardable,

        [Display(Name = "Set Group/Unit Border Zone")]
        SetGroupUnitBorderZone,

        [Display(Name = "Set Group/Unit Border Zone Return Point")]
        SetGroupUnitBorderZoneReturnPoint,

        [Display(Name = "Set Group/Unit Cloak State")]
        SetGroupUnitCloakState,

        [Display(Name = "Set Group/Unit Dockable")]
        SetGroupUnitDockable,

        [Display(Name = "Set Group/Unit Hold Position")]
        SetGroupUnitHoldPosition,

        [Display(Name = "Set Group/Unit Mission Essential")]
        SetGroupUnitMissionEssential,

        [Display(Name = "Set Group/Unit Movable")]
        SetGroupUnitMovable,

        [Display(Name = "Set Group/Unit Owner")]
        SetGroupUnitOwner,

        [Display(Name = "Set Group/Unit Towable")]
        SetGroupUnitTowable,

        [Display(Name = "Set Group/Unit Visibility")]
        SetGroupUnitVisibility,

        [Display(Name = "Set Group/Unit Vulnerability")]
        SetGroupUnitVulnerability,

        [Display(Name = "Set Group/Unit Warning Shot Mode")]
        SetGroupUnitWarningShotMode,

        [Display(Name = "Set Is In Calyan Abyss")]
        SetIsInCalyanAbyss,

        [Display(Name = "Set Lifeboat Creation State")]
        SetLifeboatCreationState,

        [Display(Name = "Set Map Text Visibility")]
        SetMapTextVisibility,

        [Display(Name = "Set Max Throttle Percent (Max user settable)")]
        SetMaxThrottlePercent,

        [Display(Name = "Set Nebula Fadeout Distance")]
        SetNebulaFadeoutDistance,

        [Display(Name = "Set Nebula to Lock Objects")]
        SetNebulaToLockObjects,

        [Display(Name = "Set Objective Task Active State")]
        SetObjectiveTaskActiveState,

        [Display(Name = "Set Objective Task Complete State")]
        SetObjectiveTaskCompleteState,

        [Display(Name = "Set Objective Task Failed State")]
        SetObjectiveTaskFailedState,

        [Display(Name = "Set Radar Active State")]
        SetRadarActiveState,

        [Display(Name = "Set Ship Banner Type")]
        SetShipBannerType,

        [Display(Name = "Set Ship Name")]
        SetShipName,

        [Display(Name = "Set Ship Top Speed Percentage")]
        SetShipTopSpeedPercentage,

        [Display(Name = "Set Ships Flag Texture")]
        SetShipsFlagTexture,

        [Display(Name = "Set alliance between PlayerA and PlayerB to TRUE/FALSE")]
        SetAllianceBetweenPlayerAToTRUEFALSE,

        [Display(Name = "SetCollidable")]
        SetCollidable,

        [Display(Name = "Setup Asteroid Belt")]
        SetupAsteroidBelt,

        [Display(Name = "Setup Space Animal Flock")]
        SetupSpaceAnimalFlock,

        [Display(Name = "Setup Team Objective")]
        SetupTeamObjective,

        [Display(Name = "Start Timer")]
        StartTimer,

        [Display(Name = "Stop Timer")]
        StopTimer,

        [Display(Name = "TUTORIAL - Pause when starmap opens?")]
        TutorialPauseWhenStarmapOpens,

        [Display(Name = "Team X Wins")]
        TeamXWins,

        [Display(Name = "Teleport Group/Unit")]
        TeleportGroupUnit,

        [Display(Name = "Toggle Island Repair When Docked")]
        ToggleIslandRepairWhenDocked,

        [Display(Name = "Tow Ship")]
        TowShip,

        [Display(Name = "Transfer Group/Unit to Group")]
        TransferGroupUnitToGroup
    }
}
