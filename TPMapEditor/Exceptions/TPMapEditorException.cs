using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Exceptions
{
    public class TPMapEditorException : Exception
    {
        public TPMapEditorException(string message) : base(message) { }
        public TPMapEditorException(string message, Exception exception) : base(message, exception) { }
    }
}
