using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public enum RuleCondition
    {
        [Display(Name = "All Units from Group have entered trigger volume Once")]
        AllUnitsFromGroupHaveEnteredTriggerVolumeOnce,

        [Display(Name = "Does Group Contain Unit Name")]
        DoesGroupContainUnitName,

        [Display(Name = "Enter Volume")]
        EnterVolume,

        [Display(Name = "Exit Volume")]
        ExitVolume,

        [Display(Name = "Flag Condition")]
        FlagCondition,

        [Display(Name = "Group Destroyed")]
        GroupDestroyed,

        [Display(Name = "Group has X members")]
        GroupHasXMembers,

        [Display(Name = "Group to Group Distance")]
        GroupToGroupDistance,

        [Display(Name = "Group/Unit Contains no mission essential ships")]
        GroupUnitContainsNoMissionEssentialShips,

        [Display(Name = "Group/Unit Docked")]
        GroupUnitDocked,

        [Display(Name = "Group/Unit Fired X Shots")]
        GroupUnitFiredXShots,

        [Display(Name = "Group/Unit Hit at least x Times")]
        GroupUnitHitAtLeastXTimes,

        [Display(Name = "Group/Unit Hit at least x Times by Player ( with eqivalence )")]
        GroupUnitHitAtLeastXTimesByPlayerWithEquivalence,

        [Display(Name = "Group/Unit Is Docked")]
        GroupUnitIsDocked,

        [Display(Name = "Group/Unit Vital Section has >,<,= X damage")]
        GroupUnitVitalSectionHasDamage,

        [Display(Name = "Group/Unit has >,<,= X damage")]
        GroupUnitHasDamage,

        [Display(Name = "Group Under Attack")]
        GroupUnderAttack,

        [Display(Name = "Is Group A attacking Group B")]
        IsGroupAAttackingGroupB,

        [Display(Name = "Is Group In Volume")]
        IsGroupInVolume,

        [Display(Name = "Is Ship In Tow")]
        IsShipInTow,

        [Display(Name = "Is Starmap Open")]
        IsStarmapOpen,

        [Display(Name = "Mission 9 - If Mortar explodes within Area")]
        Mission9IfMortarExplodesWithinArea,

        [Display(Name = "No Human Controlled Ships Remain")]
        NoHumanControlledShipsRemain,

        [Display(Name = "No Team Has Ships")]
        NoTeamHasShips,

        [Display(Name = "Player has hit Group/Unit at least x Times")]
        PlayerHasHitGroupUnitAtLeastXTimes,

        [Display(Name = "Player has no lifeboats")]
        PlayerHasNoLifeboats,

        [Display(Name = "Player Killed A Object")]
        PlayerKilledAObject,

        [Display(Name = "Player Vs Player capture count")]
        PlayerVsPlayerCaptureCount,

        [Display(Name = "Skirmish Game Complete")]
        SkirmishGameComplete,

        [Display(Name = "Speech Event Not Played Yet")]
        SpeechEventNotPlayedYet,

        [Display(Name = "Speech Event Played Once")]
        SpeechEventPlayedOnce,

        [Display(Name = "Team Game Complete")]
        TeamGameComplete,

        [Display(Name = "Team Member Enters Volume")]
        TeamMemberEntersVolume,

        [Display(Name = "Team X has no ships")]
        TeamXHasNoShips,

        [Display(Name = "Team has captured a ship from Group/Unit")]
        TeamHasCapturedAShipFromGroupUnit,

        [Display(Name = "Team has destroyed a ship from Group/Unit")]
        TeamHasDestroyedAShipFromGroupUnit,

        [Display(Name = "Team has X points")]
        TeamHasXPoints,

        [Display(Name = "Timer Condition")]
        TimerCondition,

        [Display(Name = "Unit Flag Texture")]
        UnitFlagTexture,

        [Display(Name = "Unit from Group enters trigger volume ( Once per Unit )")]
        UnitFromGroupEntersTriggerVolumeOncePerUnit,

        [Display(Name = "Unit is Within any Nebula")]
        UnitIsWithinAnyNebula,

        [Display(Name = "World Initialize")]
        WorldInitialize
    }
}
