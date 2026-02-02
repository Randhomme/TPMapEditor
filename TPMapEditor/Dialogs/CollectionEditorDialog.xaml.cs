using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for CollectionEditorDialog.xaml
    /// </summary>
    public partial class CollectionEditorDialog : DialogWindow
    {
        public CollectionEditorDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
