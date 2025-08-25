using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class RuleCondition : ObservableObject
    {
        [ObservableProperty]
        private Enums.RuleCondition type;

        partial void OnTypeChanged(Enums.RuleCondition value)
        {
            switch (value)
            {
                case Enums.RuleCondition.AllUnitsFromGroupHaveEnteredTriggerVolumeOnce:
                    break;
                case Enums.RuleCondition.DoesGroupContainUnitName:
                    break;
                case Enums.RuleCondition.EnterVolume:
                    break;
                case Enums.RuleCondition.ExitVolume:
                    break;
                case Enums.RuleCondition.FlagCondition:
                    break;
                case Enums.RuleCondition.GroupDestroyed:
                    break;
                case Enums.RuleCondition.GroupHasXMembers:
                    break;
                case Enums.RuleCondition.GroupToGroupDistance:
                    break;
                case Enums.RuleCondition.GroupUnitContainsNoMissionEssentialShips:
                    break;
                case Enums.RuleCondition.GroupUnitDocked:
                    break;
                case Enums.RuleCondition.GroupUnitFiredXShots:
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimes:
                    break;
                case Enums.RuleCondition.GroupUnitHitAtLeastXTimesByPlayer:
                    break;
                case Enums.RuleCondition.GroupUnitIsDocked:
                    break;
                case Enums.RuleCondition.GroupUnitVitalSectionHasDamage:
                    break;
                case Enums.RuleCondition.GroupUnitHasDamage:
                    break;
                case Enums.RuleCondition.GroupUnitIsWithinAnyNebula:
                    break;
                case Enums.RuleCondition.GroupUnitUnderAttack:
                    break;
                case Enums.RuleCondition.IsGroupAAttackingGroupB:
                    break;
                case Enums.RuleCondition.IsGroupInVolume:
                    break;
                case Enums.RuleCondition.IsShipInTow:
                    break;
                case Enums.RuleCondition.IsStarmapOpen:
                    break;
                case Enums.RuleCondition.Mission9IfMortarExplodesWithinArea:
                    break;
                case Enums.RuleCondition.NoHumanControlledShipsRemain:
                    break;
                case Enums.RuleCondition.NoTeamHasShips:
                    break;
                case Enums.RuleCondition.PlayerHasHitGroupUnitAtLeastXTimes:
                    break;
                case Enums.RuleCondition.PlayerHasNoLifeboats:
                    break;
                case Enums.RuleCondition.PlayerKilledAObject:
                    break;
                case Enums.RuleCondition.PlayerVsPlayerCaptureCount:
                    break;
                case Enums.RuleCondition.SkirmishGameComplete:
                    break;
                case Enums.RuleCondition.SpeechEventNotPlayedYet:
                    break;
                case Enums.RuleCondition.SpeechEventPlayedOnce:
                    break;
                case Enums.RuleCondition.TeamGameComplete:
                    break;
                case Enums.RuleCondition.TeamMemberEntersVolume:
                    break;
                case Enums.RuleCondition.TeamXHasNoShips:
                    break;
                case Enums.RuleCondition.TeamHasCapturedAShipFromGroupUnit:
                    break;
                case Enums.RuleCondition.TeamHasDestroyedAShipFromGroupUnit:
                    break;
                case Enums.RuleCondition.TeamHasXPoints:
                    break;
                case Enums.RuleCondition.TimerCondition:
                    break;
                case Enums.RuleCondition.UnitFlagTexture:
                    break;
                case Enums.RuleCondition.UnitFromGroupEntersTriggerVolumeOncePerUnit:
                    break;
                case Enums.RuleCondition.UnitIsWithinAnyNebula:
                    break;
                default:
                    break;
            }
        }
    }
}
