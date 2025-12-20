using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldObjectivePoint : RuleField<ObjectivePoint>
    {
        public RuleFieldObjectivePoint(WorldMap map, string? realLabel, string? label, ObjectivePoint value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(map, realLabel, label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return $"{RealLabel} '{Value?.ToString() ?? ObjectivePoint.DefaultName}'";
        }
    }
}
