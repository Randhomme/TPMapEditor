using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TeamsDialog.xaml
    /// </summary>
    public partial class TeamsDialog : DialogWindow
    {
        public TeamsDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
