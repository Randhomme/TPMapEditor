using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Enums
{
    /// <summary>
    /// Used by "Player Killed A Object" rule condition
    /// </summary>
    public enum WorldObjectType
    {
        Asteroid,
        Ship,
        // nothing else is killable anyway
    }
}
