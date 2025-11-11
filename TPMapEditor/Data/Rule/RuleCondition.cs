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

        private void AddRuleFieldBool(string? label = null, bool value = false, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldBool(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldDouble(string? label = null, double value = 0, double min = -9999, double max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldDouble(label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldEquivalence(string? label = null, Equivalence value = Equivalence.GreaterThan, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldEquivalence(label, value, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlag(string? label, Flag? flag = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flag ??= map.Flags.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlag(label, flag, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldFlagTexture(string? label, string? flagTexture = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            flagTexture ??= AppSettings.FlagTextures.FirstOrDefault();
            RuleFields.Add(new RuleFieldFlagTexture(label, flagTexture, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroup(string? label, Group? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroup(label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldGroupUnit(string? label = null, NamedElement? group = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            group ??= map.Groups.FirstOrDefault();
            RuleFields.Add(new RuleFieldGroupUnit(label, group, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldInt(string? label = null, int value = 0, int min = -9999, int max = 9999, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldInt(label, value, min, max, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldPlayer(string? label = null, Player? player = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            player ??= map.Players.FirstOrDefault();
            RuleFields.Add(new RuleFieldPlayer(label, player, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldSpeechEvent(string? label = null, SpeechEvent? speechEvent = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            speechEvent ??= map.SpeechEvents.FirstOrDefault();
            RuleFields.Add(new RuleFieldSpeechEvent(label, speechEvent, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTeam(string? label = null, Team? team = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            team ??= map.InGameTeams.FirstOrDefault();
            RuleFields.Add(new RuleFieldTeam(label, team, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldTimer(string? label = null, Timer? timer = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            timer ??= map.Timers.FirstOrDefault();
            RuleFields.Add(new RuleFieldTimer(label, timer, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVitalSection(string? label = null, VitalSection vitalSection = VitalSection.VitalToMission, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldVitalSection(label, vitalSection, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldVolume(string? label = null, WorldPolygon? volume = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            volume ??= map.WorldPolygons.FirstOrDefault();
            RuleFields.Add(new RuleFieldWorldPolygon(label, volume, isOptional, optionalLabel, isShown));
        }

        private void AddRuleFieldWorldObjectType(string? label = null, WorldObjectType worldObjectType = WorldObjectType.Ship, bool isOptional = false, string? optionalLabel = null, bool isShown = true)
        {
            RuleFields.Add(new RuleFieldWorldObjectType(label, worldObjectType, isOptional, optionalLabel, isShown));
        }

        partial void OnTypeChanged(Enums.RuleCondition value)
        {
            switch (value)
            {
                case Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group");
                    AddRuleFieldVolume("Volume");
                    AddRuleFieldPlayer("Player");
                    AddRuleFieldInt("Object that have already entered", isShown: false); //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName: //the name is bad, it only checks the existance of a unit
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit"); //might need to be a unit
                    AddRuleFieldBool("Exists");
                    break;
                case Enums.RuleCondition.EnterVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit"); //might need to be a unit
                    AddRuleFieldVolume("Volume");
                    break;
                case Enums.RuleCondition.ExitVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit"); //might need to be a unit
                    AddRuleFieldVolume("Volume");
                    break;
                case Enums.RuleCondition.FlagCondition:
                    RuleFields.Clear();
                    AddRuleFieldFlag("Flag");
                    AddRuleFieldBool("Value", true);
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit"); //might need to be a group ?
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldInt("Number in group", min: 0);
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit A");
                    AddRuleFieldGroupUnit("Group/Unit B");
                    AddRuleFieldInt("Distance", 100, min: 0);
                    AddRuleFieldEquivalence("Equivalence");
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    RuleFields.Clear();
                    AddRuleFieldGroup("Group"); //might be Group/Unit
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dockers");
                    AddRuleFieldGroupUnit("Targets");
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldInt("Number of rounds fired", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldInt("Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayerWithEquivalence:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPlayer("Player");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldInt("Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Dockers");
                    AddRuleFieldGroupUnit("Targets");
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldVitalSection("Vital section");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldDouble("Damage percent", min: 0, max: 1);
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldDouble("Damage percent", min: 0, max: 1);
                    break;
                case Enums.RuleCondition.GroupUnderAttack:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group A");
                    AddRuleFieldGroupUnit("Group B");
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldVolume("Volume");
                    AddRuleFieldBool("Entire group");
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Towing group");
                    AddRuleFieldGroupUnit("Target to tow"); //might need to be a unit
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    RuleFields.Clear();
                    AddRuleFieldVolume("Volume");
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldPlayer("Player");
                    AddRuleFieldInt("Number of times hit", min: 0);
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player");
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player");
                    AddRuleFieldWorldObjectType("World object type");
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    RuleFields.Clear();
                    AddRuleFieldPlayer("Player A");
                    AddRuleFieldPlayer("Player B");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldInt("Number of captures", min: 0);
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    RuleFields.Clear();
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech event");
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    RuleFields.Clear();
                    AddRuleFieldSpeechEvent("Speech event");
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldVolume("Volume");
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldInt("Number of captures", min: 0);
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldInt("Number of ships destroyed", min: 0);
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    RuleFields.Clear();
                    AddRuleFieldTeam("Team");
                    AddRuleFieldInt("Points"); //might need to be > 0
                    break;
                case Enums.RuleCondition.TimerCondition:
                    RuleFields.Clear();
                    AddRuleFieldTimer("Timer");
                    AddRuleFieldEquivalence("Equivalence");
                    AddRuleFieldInt("Time in seconds", min: 0);
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldFlagTexture("Flag texture");
                    AddRuleFieldBool("Value");
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit");
                    AddRuleFieldVolume("Volume");
                    AddRuleFieldTeam("Team");
                    AddRuleFieldInt("Number of objects that have already entered waiting to report", min: 0);
                    AddRuleFieldInt("Object that have already entered", isShown: false); //not used, must be 0 for now
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    RuleFields.Clear();
                    AddRuleFieldGroupUnit("Group/Unit"); //might need to be a unit
                    break;
                default:
                    RuleFields.Clear();
                    break;
            }
        }
    }
}
