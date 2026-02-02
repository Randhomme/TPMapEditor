using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for FlagDialog.xaml
    /// </summary>
    public partial class FlagDialog : DialogWindow
    {
        public FlagDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
