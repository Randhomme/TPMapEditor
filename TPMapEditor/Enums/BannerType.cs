using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    public enum BannerType
    {
        [Display(Name = "No Banner")]
        NoBanner,
        [Display(Name = "Banner RedTeam")]
        RedTeam,
        [Display(Name = "Banner BlueTeam")]
        BlueTeam,
        [Display(Name = "Banner BlackTeam")]
        BlackTeam
    }
}
