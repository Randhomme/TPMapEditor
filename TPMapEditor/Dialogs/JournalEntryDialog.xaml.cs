using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for JournalEntryDialog.xaml
    /// </summary>
    public partial class JournalEntryDialog : DialogWindow
    {
        public JournalEntryDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
