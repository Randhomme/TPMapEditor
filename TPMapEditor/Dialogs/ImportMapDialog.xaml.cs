using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportMapDialog.xaml
    /// </summary>
    public partial class ImportMapDialog : DialogWindow
    {
        public ImportMapDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
