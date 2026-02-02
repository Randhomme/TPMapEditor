using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TimerDialog.xaml
    /// </summary>
    public partial class TimerDialog : DialogWindow
    {
        public TimerDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
