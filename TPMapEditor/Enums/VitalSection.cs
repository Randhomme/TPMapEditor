using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public enum VitalSection
    {
        [Display(Name = "vitalToMission")]
        VitalToMission,
        [Display(Name = "vitalToShip")]
        VitalToShip,
        [Display(Name = "vitalToMaxVelocity")]
        VitalToMaxVelocity,
        [Display(Name = "vitalToManeuverability")]
        VitalToManeuverability,
    }
}
