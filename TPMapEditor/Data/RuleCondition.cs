using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class RuleCondition : ObservableObject
    {
        private readonly WorldMap map;
        [ObservableProperty]
        private Enums.RuleCondition type;
        [ObservableProperty]
        private bool? isGroupUnit1Unit, isGroupUnit2Unit;
        [ObservableProperty]
        private bool hasGroup1, hasUnit1, hasVolume1, hasPlayer1, hasPlayer2, hasFlag1, hasSpeechEvent1, hasTimer1, hasTeam1, hasEquivalence1, hasVitalSection1, hasWorldObjectType1, hasFlagType1;
        [ObservableProperty]
        private string? groupUnit1Label, groupUnit2Label, group1Label, unit1Label, volume1Label, player1Label, player2Label, flag1Label, speechEvent1Label, timer1Label, team1Label, equivalence1Label, vitalSection1Label, worldObjectType1Label, flagType1Label, bool1Label, bool2Label, int1Label, int2Label, double1Label;
        [ObservableProperty]
        private NamedElement? groupUnit1, groupUnit2, group1, unit1, volume1, player1, player2, flag1, speechEvent1, timer1;
        [ObservableProperty]
        private Team? team1;
        [ObservableProperty]
        private Enums.Equivalence equivalence1;
        [ObservableProperty]
        private Enums.VitalSection vitalSection1;
        [ObservableProperty]
        private Enums.WorldObjectType worldObjectType1;
        [ObservableProperty]
        private string? flagType1;
        [ObservableProperty]
        private bool? bool1;
        [ObservableProperty]
        private int? int1, int2, int3;
        [ObservableProperty]
        private double? double1;

        public RuleCondition(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce);
        }

        private void SetDefaults()
        {
            //objects
            GroupUnit1 = GroupUnit2 = Group1 = Unit1 = Volume1 = Player1 = Player2 = Flag1 = SpeechEvent1 = Timer1 = null;
            Team1 = null;

            //strings
            FlagType1 = null;
            GroupUnit1Label = GroupUnit2Label = Group1Label = Unit1Label = Volume1Label = Player1Label = Player2Label = Flag1Label = SpeechEvent1Label = Timer1Label = Team1Label = Equivalence1Label = VitalSection1Label = WorldObjectType1Label = FlagType1Label = Bool1Label = Bool2Label = Int1Label = Int2Label = Double1Label = string.Empty;

            //ints
            Int1 = Int2 = Int3 = null; ;

            //floats
            Double1 = null;

            //bools
            IsGroupUnit1Unit = IsGroupUnit2Unit = Bool1 = null;
            HasGroup1 = HasUnit1 = HasVolume1 = HasPlayer1 = HasPlayer2 = HasFlag1 = HasSpeechEvent1 = HasTimer1 = HasTeam1 = HasEquivalence1 = HasVitalSection1 = HasWorldObjectType1 = HasFlagType1 = false;

            //enums
            Equivalence1 = Enums.Equivalence.GreaterThan;
            VitalSection1 = Enums.VitalSection.VitalToMission;
            WorldObjectType1 = Enums.WorldObjectType.Ship;
        }

        partial void OnIsGroupUnit1UnitChanged(bool? value)
        {
            if (value != null)
                if (value == true)
                    GroupUnit1 = map.ShipUnits.FirstOrDefault();
                else
                    GroupUnit1 = map.Groups.FirstOrDefault();
            else
                GroupUnit1 = null;
        }

        partial void OnIsGroupUnit2UnitChanged(bool? value)
        {
            if (value != null)
                if (value == true)
                    GroupUnit2 = map.ShipUnits.FirstOrDefault();
                else
                    GroupUnit2 = map.Groups.FirstOrDefault();
            else
                GroupUnit2 = null;
        }

        partial void OnTypeChanged(Enums.RuleCondition value)
        {
            switch (value)
            {
                case Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce:
                    SetDefaults();
                    Group1Label = "Group";
                    Volume1Label = "Volume";
                    Player1Label = "Player";
                    HasGroup1 = HasVolume1 = HasPlayer1 = true;
                    Group1 = map.Groups.FirstOrDefault();
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    Player1 = map.Players.FirstOrDefault();
                    Int3 = 0; //ObjectThatHaveAlreadyEntered, not used, must be 0 for now
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Bool1Label = "Exists";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a unit
                    Bool1 = false;
                    break;
                case Enums.RuleCondition.EnterVolume:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Volume1Label = "Volume";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a unit
                    HasVolume1 = true;
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.ExitVolume:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Volume1Label = "Volume";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a unit
                    HasVolume1 = true;
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.FlagCondition:
                    SetDefaults();
                    Flag1Label = "Flag";
                    Bool1Label = "Value";
                    HasFlag1 = true;
                    Flag1 = map.Flags.FirstOrDefault();
                    Bool1 = true;
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a group
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    SetDefaults();
                    Group1Label = "Group";
                    Equivalence1Label = "Equivalence";
                    Int1Label = "Number in group";
                    HasGroup1 = HasEquivalence1 = true;
                    Group1 = map.Groups.FirstOrDefault();
                    Int1 = 1;
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    SetDefaults();
                    Equivalence1Label = "Equivalence";
                    HasEquivalence1 = true;
                    GroupUnit1Label = "Group A";
                    GroupUnit2Label = "Group B";
                    Int1Label = "Distance";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    GroupUnit2 = map.Groups.FirstOrDefault();
                    Int1 = 100;
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    SetDefaults();
                    Group1Label = "Group";
                    HasGroup1 = true; //might also be unit
                    Group1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    SetDefaults();
                    GroupUnit1Label = "Dockers";
                    GroupUnit2Label = "Targets";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    GroupUnit2 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Equivalence1Label = "Equivalence";
                    Int1Label = "Number of rounds fired";
                    IsGroupUnit1Unit = false;
                    HasEquivalence1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Int1Label = "Number of times hit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayerWithEquivalence:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Equivalence1Label = "Equivalence";
                    Player1Label = "Player";
                    Int1Label = "Number of times hit";
                    IsGroupUnit1Unit = false;
                    HasEquivalence1 = true;
                    HasPlayer1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Player1 = map.Players.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    SetDefaults();
                    GroupUnit1Label = "Dockers";
                    GroupUnit2Label = "Targets";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    GroupUnit2 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Equivalence1Label = "Equivalence";
                    VitalSection1Label = "Vital section";
                    Double1Label = "Damage percent";
                    IsGroupUnit1Unit = false;
                    HasEquivalence1 = HasVitalSection1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Double1 = 0;
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Equivalence1Label = "Equivalence";
                    Double1Label = "Damage percent";
                    IsGroupUnit1Unit = false;
                    HasEquivalence1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Double1 = 0;
                    break;
                case Enums.RuleCondition.GroupUnderAttack:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    SetDefaults();
                    GroupUnit1Label = "Group A";
                    GroupUnit2Label = "Group B";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    GroupUnit2 = map.Groups.FirstOrDefault();
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Volume1Label = "Volume";
                    Bool1Label = "Entire group";
                    IsGroupUnit1Unit = false;
                    HasVolume1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a unit
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    Bool1 = true;
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    SetDefaults();
                    GroupUnit1Label = "Towing group";
                    GroupUnit2Label = "Target to tow";
                    IsGroupUnit1Unit = IsGroupUnit2Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    GroupUnit2 = map.Groups.FirstOrDefault(); //might need to be a unit
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    SetDefaults();
                    Volume1Label = "Volume";
                    HasVolume1 = true;
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    Player1Label = "Player";
                    Int1Label = "Number of times hit";
                    IsGroupUnit1Unit = false;
                    HasPlayer1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Player1 = map.Players.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    SetDefaults();
                    Player1Label = "Player";
                    HasPlayer1 = true;
                    Player1 = map.Players.FirstOrDefault();
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    SetDefaults();
                    Player1Label = "Player";
                    WorldObjectType1Label = "World object type";
                    HasPlayer1 = HasWorldObjectType1 = true;
                    Player1 = map.Players.FirstOrDefault();
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    SetDefaults();
                    Player1Label = "Player A";
                    Player2Label = "Player B";
                    Equivalence1Label = "Equivalence";
                    Int1Label = "Number of captures";
                    HasPlayer1 = HasPlayer2 = true;
                    Player1 = map.Players.FirstOrDefault();
                    Player2 = map.Players.FirstOrDefault();
                    HasEquivalence1 = true;
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    SetDefaults();
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    SetDefaults();
                    SpeechEvent1Label = "Speech event";
                    HasSpeechEvent1 = true;
                    SpeechEvent1 = map.SpeechEvents.FirstOrDefault();
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    SetDefaults();
                    SpeechEvent1Label = "Speech event";
                    HasSpeechEvent1 = true;
                    SpeechEvent1 = map.SpeechEvents.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    SetDefaults();
                    Team1Label = "Team";
                    Volume1Label = "Volume";
                    HasTeam1 = HasVolume1 = true;
                    Team1 = map.Teams.FirstOrDefault();
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    SetDefaults();
                    Team1Label = "Team";
                    HasTeam1 = true;
                    Team1 = map.Teams.FirstOrDefault();
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    SetDefaults();
                    Team1Label = "Team";
                    GroupUnit1Label = "Group/Unit";
                    Int1Label = "Number of captures";
                    HasTeam1 = true;
                    Team1 = map.Teams.FirstOrDefault();
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    SetDefaults();
                    Team1Label = "Team";
                    GroupUnit1Label = "Group/Unit";
                    Int1Label = "Number of ships destroyed";
                    HasTeam1 = true;
                    IsGroupUnit1Unit = false;
                    Team1 = map.Teams.FirstOrDefault();
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    SetDefaults();
                    Team1Label = "Team";
                    Int2Label = "Points";
                    HasTeam1 = true;
                    Team1 = map.Teams.FirstOrDefault();
                    Int2 = 0;
                    break;
                case Enums.RuleCondition.TimerCondition:
                    SetDefaults();
                    Timer1Label = "Timer";
                    Equivalence1Label = "Equivalence";
                    Int1Label = "Time in seconds";
                    HasTimer1 = HasEquivalence1 = true;
                    Timer1 = map.Timers.FirstOrDefault();
                    Int1 = 0;
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    FlagType1Label = "Flag texture";
                    Bool1Label = "Value";
                    IsGroupUnit1Unit = false;
                    HasFlagType1 = true;
                    GroupUnit1 = map.Groups.FirstOrDefault();
                    FlagType1 = AppSettings.FlagTextures.FirstOrDefault();
                    Bool1 = true;
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    SetDefaults();
                    Group1Label = "Group/Unit";
                    Volume1Label = "Volume";
                    Team1Label = "Team";
                    Int1Label = "Number of objects that have already entered waiting to report";
                    HasGroup1 = HasVolume1 = HasTeam1 = true;
                    Group1 = map.Groups.FirstOrDefault();
                    Volume1 = map.WorldPolygons.FirstOrDefault();
                    Team1 = map.Teams.FirstOrDefault();
                    Int1 = 0;
                    Int3 = 0; //ObjectThatHaveAlreadyEntered, not used, must be 0 for now
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    SetDefaults();
                    GroupUnit1Label = "Group/Unit";
                    IsGroupUnit1Unit = false;
                    GroupUnit1 = map.Groups.FirstOrDefault(); //might need to be a unit
                    break;
                default:
                    SetDefaults();
                    break;
            }
        }
    }
}
