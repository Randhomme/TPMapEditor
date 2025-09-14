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
        private bool? isGroupUnitUnit, isDockersUnit, isTargetsUnit, isGroupAUnit, isGroupBUnit, isTowingGroupUnit, isTargetToTowUnit;
        [ObservableProperty]
        private bool hasGroup, hasVolume, hasPlayer, hasPlayerA, hasPlayerB, hasFlag, hasEquivalence, hasVitalSection, hasWorldObjectType, hasSpeechEvent, hasTeam, hasTimer, hasFlagType;
        [ObservableProperty]
        private string? flagType;
        [ObservableProperty]
        private NamedElement? groupUnit, dockers, targets, groupA, groupB, towingGroup, targetToTow; //group/unit
        [ObservableProperty]
        private NamedElement? flag, speechEvent, volume, player, playerA, playerB, timer;
        [ObservableProperty]
        private NamedElement? group; //group
        [ObservableProperty]
        private NamedElement? unit; //unit
        [ObservableProperty]
        private Team? team;
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
            //objects
            GroupUnit = Dockers = Targets = GroupA = GroupB = TowingGroup = TargetToTow = Flag = SpeechEvent = Volume = Player = PlayerA = PlayerB = Timer = Group = Unit = null;
            Team = null;

            //strings
            FlagType = null;

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
            IsGroupUnitUnit = IsDockersUnit = IsTargetsUnit = IsGroupAUnit = IsGroupBUnit = IsTowingGroupUnit = IsTargetToTowUnit = null;
            HasGroup = HasVolume = HasPlayer = HasPlayerA = HasPlayerB = HasFlag = HasEquivalence = HasVitalSection = HasWorldObjectType = HasSpeechEvent = HasTeam = HasTimer = HasFlagType = false;

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
                    HasGroup = HasVolume = HasPlayer = true;
                    Group = map.Groups.FirstOrDefault();
                    Volume = map.WorldPolygons.FirstOrDefault();
                    Player = map.Players.FirstOrDefault();
                    ObjectThatHaveAlreadyEntered = 0; //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a unit
                    Exists = false;
                    break;
                case Enums.RuleCondition.EnterVolume:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a unit
                    HasVolume = true;
                    Volume = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.ExitVolume:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a unit
                    HasVolume = true;
                    Volume = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.FlagCondition:
                    SetDefaults();
                    HasFlag = true;
                    Flag = map.Flags.FirstOrDefault();
                    BoolValue = true;
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a group
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    SetDefaults();
                    HasGroup = HasEquivalence = true;
                    Group = map.Groups.FirstOrDefault();
                    NumberInGroup = 1;
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    SetDefaults();
                    HasEquivalence = true;
                    IsGroupAUnit = IsGroupBUnit = false;
                    GroupA = map.Groups.FirstOrDefault();
                    GroupB = map.Groups.FirstOrDefault();
                    Distance = 100;
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    SetDefaults();
                    HasGroup = true; //might also be unit
                    Group = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    SetDefaults();
                    IsDockersUnit = IsTargetsUnit = false;
                    Dockers = map.Groups.FirstOrDefault();
                    Targets = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    HasEquivalence = true;
                    GroupUnit = map.Groups.FirstOrDefault();
                    NumberOfRoundsFired = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    NumberOfTimesHit = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayerWithEquivalence:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    HasEquivalence = true;
                    GroupUnit = map.Groups.FirstOrDefault();
                    NumberOfTimesHit = 0;
                    Player = map.Players.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    SetDefaults();
                    IsDockersUnit = IsTargetsUnit = false;
                    Dockers = map.Groups.FirstOrDefault();
                    Targets = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    HasEquivalence = HasVitalSection = true;
                    GroupUnit = map.Groups.FirstOrDefault();
                    DamagePercent = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    HasEquivalence = true;
                    GroupUnit = map.Groups.FirstOrDefault();
                    DamagePercent = 0;
                    break;
                case Enums.RuleCondition.GroupUnderAttack:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    SetDefaults();
                    IsGroupAUnit = IsGroupBUnit = false;
                    GroupA = map.Groups.FirstOrDefault();
                    GroupB = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a unit
                    HasVolume = true;
                    EntireGroup = true;
                    Volume = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    SetDefaults();
                    IsTowingGroupUnit = IsTargetToTowUnit = false;
                    TowingGroup = map.Groups.FirstOrDefault();
                    TargetToTow = map.Groups.FirstOrDefault(); //might need to be a unit
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    SetDefaults();
                    HasVolume = true;
                    Volume = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    HasPlayer = true;
                    Player = map.Players.FirstOrDefault();
                    NumberOfTimesHit = 0;
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    SetDefaults();
                    HasPlayer = true;
                    Player = map.Players.FirstOrDefault();
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    SetDefaults();
                    HasPlayer = HasWorldObjectType = true;
                    Player = map.Players.FirstOrDefault();
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    SetDefaults();
                    HasPlayerA = HasPlayerB = true;
                    PlayerA = map.Players.FirstOrDefault();
                    PlayerB = map.Players.FirstOrDefault();
                    HasEquivalence = true;
                    NumberOfCaptures = 0;
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    SetDefaults();
                    HasSpeechEvent = true;
                    SpeechEvent = map.SpeechEvents.FirstOrDefault();
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    SetDefaults();
                    HasSpeechEvent = true;
                    SpeechEvent = map.SpeechEvents.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    SetDefaults();
                    HasTeam = HasVolume = true;
                    Team = map.Teams.FirstOrDefault();
                    Volume = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    SetDefaults();
                    HasTeam = true;
                    Team = map.Teams.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    SetDefaults();
                    HasTeam = true;
                    Team = map.Teams.FirstOrDefault();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    NumberOfCaptures = 0;
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    SetDefaults();
                    HasTeam = true;
                    Team = map.Teams.FirstOrDefault();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    NumberOfShipsDestroyed = 0;
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    SetDefaults();
                    HasTeam = true;
                    Team = map.Teams.FirstOrDefault();
                    Points = 0;
                    break;
                case Enums.RuleCondition.TimerCondition:
                    SetDefaults();
                    HasTimer = HasEquivalence = true;
                    Timer = map.Timers.FirstOrDefault();
                    TimeInSeconds = 0;
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault();
                    HasFlagType = true;
                    FlagType = AppSettings.FlagTextures.FirstOrDefault();
                    BoolValue = true;
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    SetDefaults();
                    HasGroup = HasVolume = HasTeam = true;
                    Group = map.Groups.FirstOrDefault();
                    Volume = map.WorldPolygons.FirstOrDefault();
                    Team = map.Teams.FirstOrDefault();
                    NumberOfObjectsThatHaveAlreadyEnteredWaitingToReport = 0;
                    ObjectThatHaveAlreadyEntered = 0; //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    SetDefaults();
                    IsGroupUnitUnit = false;
                    GroupUnit = map.Groups.FirstOrDefault(); //might need to be a unit
                    break;
                default:
                    SetDefaults();
                    break;
            }
        }
    }
}
