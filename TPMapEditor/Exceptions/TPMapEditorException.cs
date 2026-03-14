using System;

namespace TPMapEditor.Exceptions
{
    public class TPMapEditorException : Exception
    {
        public TPMapEditorException(string message) : base(message) { }
        public TPMapEditorException(string message, Exception exception) : base(message, exception) { }
    }
}
