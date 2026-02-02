using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services.Implementations
{
    public class CopyPasteService : ObservableObject, ICopyPasteService
    {
        private readonly IList<ICopiableMapObject> clipboard;
        private int clipboardCount = 0;
        public int ClipboardCount { get => clipboardCount; private set => SetProperty(ref clipboardCount, value); }

        public CopyPasteService()
        {
            clipboard = new List<ICopiableMapObject>();
        }

        public void Copy<T>(IEnumerable<T> values) where T : ICopiableMapObject
        {
            ClearClipboard();
            foreach (var item in values)
            {
                clipboard.Add(item);
            }
            ClipboardCount = clipboard.Count;
        }

        public IEnumerable<T> Paste<T>() where T : ICopiableMapObject
        {
            return clipboard.Select((o)=>o.Copy()).Cast<T>().ToList();
        }

        public void ClearClipboard()
        {
            clipboard.Clear();
            ClipboardCount = 0;
        }
    }
}
