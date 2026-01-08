using System;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class CopyPasteService : ICopyPasteService
    {
        private readonly IList<ISelectableMapObject> clipboard;

        public CopyPasteService()
        {
            clipboard = new List<ISelectableMapObject>();
        }

        public void Copy<T>(IEnumerable<T> values) where T : ISelectableMapObject
        {
            ClearClipboard();
            foreach (var item in values)
            {
                clipboard.Add(item.Copy());
            }
        }

        public IEnumerable<T> Paste<T>() where T : ISelectableMapObject
        {
            return clipboard.OfType<T>();
        }

        public void ClearClipboard()
        {
            clipboard.Clear();
        }
    }
}
