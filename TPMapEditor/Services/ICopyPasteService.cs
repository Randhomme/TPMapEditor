using System.Collections.Generic;
using System.ComponentModel;
using TPMapEditor.Interfaces;

namespace TPMapEditor.Services
{
    public interface ICopyPasteService : INotifyPropertyChanged
    {
        public int ClipboardCount { get; }
        public void Copy<T>(IEnumerable<T> values) where T : ICopiableMapObject;
        public IEnumerable<T> Paste<T>() where T : ICopiableMapObject;
        public void ClearClipboard();
    }
}
