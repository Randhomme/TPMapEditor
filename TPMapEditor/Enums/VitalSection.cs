using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    public enum VitalSection
    {
        [Description("vitalToMission")]
        VitalToMission,
        [Description("vitalToShip")]
        VitalToShip,
        [Description("vitalToMaxVelocity")]
        VitalToMaxVelocity,
        [Description("vitalToManeuverability")]
        VitalToManeuverability,
    }
}
