using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data.Rule
{
    public class RuleFieldShipUnitName : RuleField<string>
    {
        public RuleFieldShipUnitName(string? realLabel, string? label, ShipUnit unit, string? value = null, bool isOptional = false, string? optionalLabel = null, bool isShown = true) : base(realLabel, label, value, isOptional, optionalLabel, isShown)
        {
            Value = unit.Name;
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Value))
                {
                    unit.Name = Value;
                }
            };
        }
    }
}
