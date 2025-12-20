using System;
using System.Collections.Generic;
using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for CollectionEditorDialog.xaml
    /// </summary>
    public partial class CollectionEditorDialog : DialogWindow
    {
        public IEnumerable<object> ItemsSource { get; }
        public Func<object> Factory { get; }
        public bool GridOnlyMode { get; }
        public CollectionEditorDialog(Window owner, string title, IEnumerable<object> itemSource, Func<object> factory, bool gridOnlyMode = false) : base(owner, title)
        {
            this.ItemsSource = itemSource;
            this.Factory = factory;
            this.GridOnlyMode = gridOnlyMode;
            InitializeComponent();
        }
    }
}
