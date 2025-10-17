using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldAiStance : RuleField<AiStance>
    {
        public RuleFieldAiStance(string? label = null, AiStance value = AiStance.AISTANCE, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }

        public override string ToString()
        {
            return Value.GetName();
        }
    }
}
