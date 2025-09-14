using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    /// <summary>
    /// Class representing a unit in world rules
    /// </summary>
    public class ShipUnit : NamedElement
    {
        public static Dictionary<string, string> ShipNamesDictionnary = new Dictionary<string, string>();
        public ShipUnit(WorldMap map, string name) : base(map, name)
        {
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.ShipUnits)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
