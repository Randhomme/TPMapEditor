using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    public enum AiStance
    {
        [Display(Name = "AI STANCE")]
        AISTANCE,
        [Display(Name = "Stance Persistant")]
        StancePersistant,
        [Display(Name = "Stance_Aggressive")]
        StanceAggressive,
        [Display(Name = "Stance_Dummy")]
        StanceDummy,
        [Display(Name = "Stance_Neutral")]
        StanceNeutral,
        [Display(Name = "Stance_Defensive")]
        StanceDefensive,
        [Display(Name = "Captain_Human")]
        CaptainHuman,
        [Display(Name = "Captain_Neutral")]
        CaptainNeutral,
        [Display(Name = "Captain_Opportunistic")]
        CaptainOpportunistic,
        [Display(Name = "Captain_Brave")]
        CaptainBrave,
        [Display(Name = "Captain_Supportive")]
        CaptainSupportive,
        [Display(Name = "Captain_Cautious")]
        CaptainCautious,
        [Display(Name = "Captain_BodyGuard")]
        CaptainBodyGuard,
        [Display(Name = "Captain_GungHo")]
        CaptainGungHo,
    }
}
