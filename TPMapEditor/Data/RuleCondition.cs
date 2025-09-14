using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class RuleCondition : ObservableObject
    {
        private WorldMap map;
        [ObservableProperty]
        private Enums.RuleCondition type;
        [ObservableProperty]
        private bool? isGroupUnitNameUnit, isDockersNameUnit, isTargetsNameUnit, isGroupANameUnit, isGroupBNameUnit, isTowingGroupNameUnit, isTargetToTowNameUnit;
        [ObservableProperty]
        private bool hasGroupName, hasVolumeName, hasPlayerName, hasPlayerNameA, hasPlayerNameB, hasFlagName, hasEquivalence, hasVitalSection, hasWorldObjectType, hasSpeechEventName, hasTeamName, hasTimerName, hasFlagType;
        [ObservableProperty]
        private string? groupUnitName, dockersName, targetsName, groupAName, groupBName, towingGroupName, targetToTowName; //group/unit
        [ObservableProperty]
        private string? flagName, speechEventName, volumeName, playerName, playerNameA, playerNameB, teamName, timerName, flagType;
        [ObservableProperty]
        private string? groupName; //group
        [ObservableProperty]
        private string? unitName; //unit
        [ObservableProperty]
        private int? objectThatHaveAlreadyEntered; //will always be 0, I don't know how this works yet
        [ObservableProperty]
        private int? numberOfRoundsFired, numberOfTimesHit, numberInGroup, distance, numberOfCaptures, numberOfShipsDestroyed, points, timeInSeconds, numberOfObjectsThatHaveAlreadyEnteredWaitingToReport;
        [ObservableProperty]
        private double? damagePercent;
        [ObservableProperty]
        private bool? exists, boolValue, entireGroup;
        [ObservableProperty]
        private Enums.Equivalence equivalence;
        [ObservableProperty]
        private Enums.VitalSection vitalSection;
        [ObservableProperty]
        private Enums.WorldObjectType worldObjectType;

        public RuleCondition(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce);
        }

        private void SetDefaults()
        {
            //strings
            DockersName = null;
            FlagName = null;
            FlagType = null;
            GroupAName = null;
            GroupBName = null;
            GroupName = null;
            GroupUnitName = null;
            PlayerName = null;
            PlayerNameA = null;
            PlayerNameB = null;
            SpeechEventName = null;
            TargetsName = null;
            TargetToTowName = null;
            TimerName = null;
            TeamName = null;
            TowingGroupName = null;
            UnitName = null;
            VolumeName = null;

            //ints
            ObjectThatHaveAlreadyEntered = null;
            NumberOfRoundsFired = null;
            NumberOfTimesHit = null;
            NumberInGroup = null;
            Distance = null;
            NumberOfCaptures = null;
            NumberOfShipsDestroyed = null;
            Points = null;
            TimeInSeconds = null;
            NumberOfObjectsThatHaveAlreadyEnteredWaitingToReport = null;

            //floats
            DamagePercent = null;

            //bools
            BoolValue = null;
            Exists = null;
            EntireGroup = null;
            IsGroupUnitNameUnit = IsDockersNameUnit = IsTargetsNameUnit = IsGroupANameUnit = IsGroupBNameUnit = IsTowingGroupNameUnit = IsTargetToTowNameUnit = null;
            HasGroupName = HasVolumeName = HasPlayerName = HasPlayerNameA = HasPlayerNameB = HasFlagName = HasEquivalence = HasVitalSection = HasWorldObjectType = HasSpeechEventName = HasTeamName = HasTimerName = HasFlagType = false;

            //enums
            Equivalence = Enums.Equivalence.GreaterThan;
            VitalSection = Enums.VitalSection.VitalToMission;
            WorldObjectType = Enums.WorldObjectType.Ship;
        }

        partial void OnTypeChanged(Enums.RuleCondition value)
        {
            switch (value)
            {
                case Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce:
                    SetDefaults();
                    HasGroupName = HasVolumeName = HasPlayerName = true;
                    GroupName = map.Groups.FirstOrDefault()?.Name;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    PlayerName = map.Players.FirstOrDefault()?.Name;
                    ObjectThatHaveAlreadyEntered = 0; //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    Exists = false;
                    break;
                case Enums.RuleCondition.EnterVolume:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    HasVolumeName = true;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.ExitVolume:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    HasVolumeName = true;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.FlagCondition:
                    SetDefaults();
                    HasFlagName = true;
                    FlagName = map.Flags.FirstOrDefault()?.Name;
                    BoolValue = true;
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a group
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    SetDefaults();
                    HasGroupName = HasEquivalence = true;
                    GroupName = map.Groups.FirstOrDefault()?.Name;
                    NumberInGroup = 1;
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    SetDefaults();
                    HasEquivalence = true;
                    IsGroupANameUnit = IsGroupBNameUnit = false;
                    GroupAName = map.Groups.FirstOrDefault()?.Name;
                    GroupBName = map.Groups.FirstOrDefault()?.Name;
                    Distance = 100;
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    SetDefaults();
                    HasGroupName = true; //might also be unit
                    GroupName = map.Groups.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    SetDefaults();
                    IsDockersNameUnit = IsTargetsNameUnit = false;
                    DockersName = map.Groups.FirstOrDefault()?.Name;
                    TargetsName = map.Groups.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    HasEquivalence = true;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    NumberOfRoundsFired = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    NumberOfTimesHit = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayerWithEquivalence:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    HasEquivalence = true;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    NumberOfTimesHit = 0;
                    PlayerName = map.Players.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    SetDefaults();
                    IsDockersNameUnit = IsTargetsNameUnit = false;
                    DockersName = map.Groups.FirstOrDefault()?.Name;
                    TargetsName = map.Groups.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    HasEquivalence = HasVitalSection = true;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    DamagePercent = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    HasEquivalence = true;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    DamagePercent = 0;
                    break;
                case Enums.RuleCondition.GroupUnderAttack:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    SetDefaults();
                    IsGroupANameUnit = IsGroupBNameUnit = false;
                    GroupAName = map.Groups.FirstOrDefault()?.Name;
                    GroupBName = map.Groups.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    HasVolumeName = true;
                    EntireGroup = true;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    SetDefaults();
                    IsTowingGroupNameUnit = IsTargetToTowNameUnit = false;
                    TowingGroupName = map.Groups.FirstOrDefault()?.Name;
                    TargetToTowName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    SetDefaults();
                    HasVolumeName = true;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    HasPlayerName = true;
                    PlayerName = map.Players.FirstOrDefault()?.Name;
                    NumberOfTimesHit = 0;
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    SetDefaults();
                    HasPlayerName = true;
                    PlayerName = map.Players.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    SetDefaults();
                    HasPlayerName = HasWorldObjectType = true;
                    PlayerName = map.Players.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    SetDefaults();
                    HasPlayerNameA = HasPlayerNameB = true;
                    PlayerNameA = map.Players.FirstOrDefault()?.Name;
                    PlayerNameB = map.Players.FirstOrDefault()?.Name;
                    HasEquivalence = true;
                    NumberOfCaptures = 0;
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    SetDefaults();
                    HasSpeechEventName = true;
                    SpeechEventName = map.SpeechEvents.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    SetDefaults();
                    HasSpeechEventName = true;
                    SpeechEventName = map.SpeechEvents.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    SetDefaults();
                    HasTeamName = HasVolumeName = true;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    SetDefaults();
                    HasTeamName = true;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    SetDefaults();
                    HasTeamName = true;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    NumberOfCaptures = 0;
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    SetDefaults();
                    HasTeamName = true;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    NumberOfShipsDestroyed = 0;
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    SetDefaults();
                    HasTeamName = true;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    Points = 0;
                    break;
                case Enums.RuleCondition.TimerCondition:
                    SetDefaults();
                    HasTimerName = HasEquivalence = true;
                    TimerName = map.Timers.FirstOrDefault()?.Name;
                    TimeInSeconds = 0;
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name;
                    HasFlagType = true;
                    FlagType = AppSettings.FlagTextures.FirstOrDefault();
                    BoolValue = true;
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    SetDefaults();
                    HasGroupName = HasVolumeName = HasTeamName = true;
                    GroupName = map.Groups.FirstOrDefault()?.Name;
                    VolumeName = map.WorldPolygons.FirstOrDefault()?.Name;
                    TeamName = map.Teams.FirstOrDefault()?.RealName;
                    NumberOfObjectsThatHaveAlreadyEnteredWaitingToReport = 0;
                    ObjectThatHaveAlreadyEntered = 0; //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    SetDefaults();
                    IsGroupUnitNameUnit = false;
                    GroupUnitName = map.Groups.FirstOrDefault()?.Name; //might need to be a unit
                    break;
                default:
                    break;
            }
        }
    }
}
