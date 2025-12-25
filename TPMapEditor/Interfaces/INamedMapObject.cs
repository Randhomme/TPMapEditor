using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Represents an object within the map that has a name.
    /// </summary>
    public interface INamedMapObject
    {
        public string Name { get; set; }
    }
}
