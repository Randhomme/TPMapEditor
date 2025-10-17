using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldMapTextPoint : RuleField<MapTextPoint>
    {
        public RuleFieldMapTextPoint(string? label, MapTextPoint value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
