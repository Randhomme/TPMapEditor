using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.ViewModel
{
    public class CollectionEditorViewModel : ObservableObject
    {
        public IEnumerable<object> ItemsSource { get; }
        public Func<object> Factory { get; }
        public bool GridOnlyMode { get; }

        public CollectionEditorViewModel(IEnumerable<object> itemSource, Func<object> factory, bool gridOnlyMode = false)
        {
            this.ItemsSource = itemSource;
            this.Factory = factory;
            this.GridOnlyMode = gridOnlyMode;
            OnPropertyChanged(nameof(Factory));
        }
    }
}
