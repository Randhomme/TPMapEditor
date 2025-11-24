using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Ink;
using TPMapEditor.Enums;
using TPMapEditor.Settings;

namespace TPMapEditor.Data.Rule
{
    public partial class RuleCondition : ObservableObject
    {
        private readonly WorldMap map;
        [ObservableProperty]
        private Enums.RuleCondition type;

        public ObservableCollection<RuleField> RuleFields { get; } = new();

        public RuleCondition(WorldMap map)
        {
            this.map = map;
            OnTypeChanged(Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce);
        }

        private void AddRuleFieldBool(string? realLabel, string? label, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBool(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDouble(string? realLabel, string? label, double value = 0, double min = -9999, double max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldDouble(realLabel, label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEquivalence(string? realLabel, string? label, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldEquivalence(realLabel, label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlag(string? realLabel, string? label, Flag? flag = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flag ??= map.Flags.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlag(realLabel, label, flag, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlagTexture(string? realLabel, string? label, string? flagTexture = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flagTexture ??= AppSettings.FlagTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlagTexture(realLabel, label, flagTexture, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroup(string? realLabel, string? label, Group? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroup(realLabel, label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroupUnit(string? realLabel, string? label, NamedElement? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroupUnit(realLabel, label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInt(string? realLabel, string? label, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldInt(realLabel, label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPlayer(string? realLabel, string? label, Player? player = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            player ??= map.Players.FirstOrDefault();
            RuleFields.Add(new RuleFieldPlayer(realLabel, label, player, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldSpeechEvent(string? realLabel, string? label, SpeechEvent? speechEvent = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            speechEvent ??= map.SpeechEvents.FirstOrDefault();
            RuleFields.Add(new RuleFieldSpeechEvent(realLabel, label, speechEvent, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTeam(string? realLabel, string? label, Team? team = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            team ??= map.InGameTeams.FirstOrDefault();
            RuleFields.Add(new RuleFieldTeam(realLabel, label, team, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTimer(string? realLabel, string? label, Timer? timer = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            timer ??= map.Timers.FirstOrDefault();
            RuleFields.Add(new RuleFieldTimer(realLabel, label, timer, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVitalSection(string? realLabel, string? label, VitalSection vitalSection = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldVitalSection(realLabel, label, vitalSection, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVolume(string? realLabel, string? label, WorldPolygon? volume = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            volume ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(realLabel, label, volume, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldObjectType(string? realLabel, string? label, KillableWorldObjectType worldObjectType = KillableWorldObjectType.Ship, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldWorldObjectType(realLabel, label, worldObjectType, isOptional, optionalLabel, isShown));
        }

        partial void OnTypeChanged(Enums.RuleCondition value)
        {
            switch (value)
            {
                case Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group Name String","Group");
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    AddRuleFieldPlayer("Player Name String", "Player");
                    AddRuleFieldInt("Object that have already entered", "Object that have already entered", isShown: false); //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName: //the name is bad, it only checks the existance of a unit
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Unit Name String", "Group/Unit"); //might need to be a unit
                    AddRuleFieldBool("Exists? String", "Exists");
                    break;
                case Enums.RuleCondition.EnterVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit"); //might need to be a unit
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    break;
                case Enums.RuleCondition.ExitVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit"); //might need to be a unit
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    break;
                case Enums.RuleCondition.FlagCondition:
                    RuleFields.Clear();
                    AddRuleFieldFlag("Flag Name String", "Flag");
                    AddRuleFieldBool("Boolean Value String", "Value", true);
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit"); //might need to be a group ?
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group Name String", "Group");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldInt("number Int", "Number in group", min: 0);
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("GroupA Name String", "Group/Unit A");
                    AddRuleFieldGroupUnit("GroupB Name String", "Group/Unit B");
                    AddRuleFieldInt("Distance Int", "Distance", 100, min: 0);
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group Name String", "Group"); //might be Group/Unit
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dockers Group/Unit Name String", "Dockers");
                    AddRuleFieldGroupUnit("Targets Group/Unit Name String", "Targets");
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldInt("Number Of Rounds Fired Int", "Number of rounds fired", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldInt("Number Of Times Hit Int", "Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayerWithEquivalence:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldPlayer("Player Name String", "Player");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldInt("Number Of Times Hit Int", "Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dockers Group/Unit Name String", "Dockers");
                    AddRuleFieldGroupUnit("Targets Group/Unit Name String", "Targets");
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldVitalSection("Vital Section String", "Vital section");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldDouble("Damage Percent Float", "Damage percent", min: 0, max: 1);
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldDouble("Damage Percent Float", "Damage percent", min: 0, max: 1);
                    break;
                case Enums.RuleCondition.GroupUnderAttack:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("GroupA Name String", "Group A");
                    AddRuleFieldGroupUnit("GroupB Name String", "Group B");
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    AddRuleFieldBool("Entire Group String", "Entire group");
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Towing Group String", "Towing group");
                    AddRuleFieldGroupUnit("Target to Tow String", "Target to tow"); //might need to be a unit
                    AddRuleFieldBool("Boolean String", "Value", value: true);
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    RuleFields.Clear();
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit Name String", "Group/Unit");
                    AddRuleFieldPlayer("Player Name String", "Player");
                    AddRuleFieldInt("Number Of Times Hit", "Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player Name String", "Player");
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player Name String", "Player");
                    AddRuleFieldWorldObjectType("World Object Type String", "World object type");
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player Name A String", "Player A");
                    AddRuleFieldPlayer("Player Name B", "Player B");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldInt("Number Of Captures", "Number of captures", min: 0);
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech Event Name", "Speech event");
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech Event Name String", "Speech event");
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldGroupUnit("Group/Unit String", "Group/Unit");
                    AddRuleFieldInt("Number Of Captures Int", "Number of captures", min: 0);
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldGroupUnit("Group/Unit String", "Group/Unit");
                    AddRuleFieldInt("Number Of Ships Destroyed Int", "Number of ships destroyed", min: 0);
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldInt("Points Int", "Points"); //might need to be > 0
                    break;
                case Enums.RuleCondition.TimerCondition:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer Name String", "Timer");
                    AddRuleFieldEquivalence("Equivalence String", "Equivalence");
                    AddRuleFieldInt("Time in seconds Int", "Time in seconds", min: 0);
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Unit Name String", "Group/Unit");
                    AddRuleFieldFlagTexture("Flag Type String", "Flag texture");
                    AddRuleFieldBool("Boolean String", "Value");
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group Name String", "Group/Unit");
                    AddRuleFieldVolume("Volume Name String", "Volume");
                    AddRuleFieldTeam("Team Name String", "Team");
                    AddRuleFieldInt("Number of objects entered, waiting to report Int", "Number of objects that have already entered waiting to report", min: 0);
                    AddRuleFieldInt("Object that have already entered - Size Int", "Object that have already entered", isShown: false); //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Unit Name String", "Group/Unit"); //might need to be a unit
                    break;
                case Enums.RuleCondition.WorldInitialize:
                    RuleFields.Clear();
                    break;
                default:
                    RuleFields.Clear();
                    break;
            }
        }
    }
}
