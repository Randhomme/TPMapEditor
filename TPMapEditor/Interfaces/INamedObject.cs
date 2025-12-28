using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Represents an object that has a unique name.
    /// </summary>
    public interface INamedObject
    {
        public string Name { get; set; }
    }
}
