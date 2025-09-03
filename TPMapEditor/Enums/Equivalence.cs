using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    public enum Equivalence
    {
        [Description("Equal To")]
        EqualTo,
        [Description("Greater Than")]
        GreaterThan,
        [Description("Less Than")]
        LessThan
    }
}
