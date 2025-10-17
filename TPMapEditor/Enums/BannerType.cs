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
        RedTeam,
        BlueTeam,
        BlackTeam
    }
}
