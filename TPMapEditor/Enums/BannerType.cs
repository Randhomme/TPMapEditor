using System.ComponentModel.DataAnnotations;

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
