using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TPMapEditor.Enums
{
    public enum RuleCondition
    {
        [Description("All Units from Group have entered trigger volume Once")]
        AllUnitsFromGroupHaveEnteredTriggerVolumeOnce,

        [Description("Does Group Contain Unit Name")]
        DoesGroupContainUnitName,

        [Description("Enter Volume")]
        EnterVolume,

        [Description("Exit Volume")]
        ExitVolume,

        [Description("Flag Condition")]
        FlagCondition,

        [Description("Group Destroyed")]
        GroupDestroyed,

        [Description("Group has X members")]
        GroupHasXMembers,

        [Description("Group to Group Distance")]
        GroupToGroupDistance,

        [Description("Group/Unit Contains no mission essential ships")]
        GroupUnitContainsNoMissionEssentialShips,

        [Description("Group/Unit Docked")]
        GroupUnitDocked,

        [Description("Group/Unit Fired X Shots")]
        GroupUnitFiredXShots,

        [Description("Group/Unit Hit at least x Times")]
        GroupUnitHitAtLeastXTimes,

        [Description("Group/Unit Hit at least x Times by Player (with equivalence)")]
        GroupUnitHitAtLeastXTimesByPlayer,

        [Description("Group/Unit Is Docked")]
        GroupUnitIsDocked,

        [Description("Group/Unit Vital Section has >,<,= X damage")]
        GroupUnitVitalSectionHasDamage,

        [Description("Group/Unit has >,<,= X damage")]
        GroupUnitHasDamage,

        [Description("Group/Unit is Within any Nebula")]
        GroupUnitIsWithinAnyNebula,

        [Description("Group/Unit Under Attack")]
        GroupUnitUnderAttack,

        [Description("Is Group A attacking Group B")]
        IsGroupAAttackingGroupB,

        [Description("Is Group In Volume")]
        IsGroupInVolume,

        [Description("Is Ship In Tow")]
        IsShipInTow,

        [Description("Is Starmap Open")]
        IsStarmapOpen,

        [Description("Mission 9 - If Mortar explodes within Area")]
        Mission9IfMortarExplodesWithinArea,

        [Description("No Human Controlled Ships Remain")]
        NoHumanControlledShipsRemain,

        [Description("No Team Has Ships")]
        NoTeamHasShips,

        [Description("Player has hit Group/Unit at least x Times")]
        PlayerHasHitGroupUnitAtLeastXTimes,

        [Description("Player has no lifeboats")]
        PlayerHasNoLifeboats,

        [Description("Player Killed A Object")]
        PlayerKilledAObject,

        [Description("Player Vs Player capture count")]
        PlayerVsPlayerCaptureCount,

        [Description("Skirmish Game Complete")]
        SkirmishGameComplete,

        [Description("Speech Event Not Played Yet")]
        SpeechEventNotPlayedYet,

        [Description("Speech Event Played Once")]
        SpeechEventPlayedOnce,

        [Description("Team Game Complete")]
        TeamGameComplete,

        [Description("Team Member Enters Volume")]
        TeamMemberEntersVolume,

        [Description("Team X has no ships")]
        TeamXHasNoShips,

        [Description("Team has captured a ship from Group/Unit")]
        TeamHasCapturedAShipFromGroupUnit,

        [Description("Team has destroyed a ship from Group/Unit")]
        TeamHasDestroyedAShipFromGroupUnit,

        [Description("Team has X points")]
        TeamHasXPoints,

        [Description("Timer Condition")]
        TimerCondition,

        [Description("Unit Flag Texture")]
        UnitFlagTexture,

        [Description("Unit from Group enters trigger volume (Once per Unit)")]
        UnitFromGroupEntersTriggerVolumeOncePerUnit,

        [Description("Unit is Within any Nebula")]
        UnitIsWithinAnyNebula
    }
}
