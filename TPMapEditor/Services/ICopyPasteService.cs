using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services
{
    public interface ICopyPasteService
    {
        public void Copy<T>(IEnumerable<T> values) where T : ISelectableMapObject;
        public IEnumerable<T> Paste<T>() where T : ISelectableMapObject;
        public void ClearClipboard();
    }
}
