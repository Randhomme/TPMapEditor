using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Enums;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldFormationType : RuleField<FormationType>
    {
        public RuleFieldFormationType(string? label, FormationType value, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(label, value, isOptional, optionalLabel, isShown)
        {
        }
    }
}
