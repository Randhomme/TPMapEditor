using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PlayerAllianceWindow.xaml
    /// </summary>
    public partial class PlayerAllianceDialog : DialogWindow
    {
        public PlayerAllianceDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
