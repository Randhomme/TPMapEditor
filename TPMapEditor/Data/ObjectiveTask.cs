using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public class ObjectiveTask : NamedElement
    {
        public ObjectiveTask(WorldMap map, string name) : base(map, name)
        {
        }

        protected override bool IsNameTaken(string name)
        {
            throw new NotImplementedException();
        }
    }
}
