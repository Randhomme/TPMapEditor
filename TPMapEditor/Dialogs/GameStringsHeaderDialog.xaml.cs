using System;
using System.Windows;
using TPMapEditor.Data;
using TPMapEditor.Settings;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for GameHeadersDialog.xaml
    /// </summary>
    public partial class GameStringsHeadersDialog : DialogWindow
    {
        public AppSettings AppSettings { get; }
        public Func<GameHeadersFile> Factory { get; }
        public GameStringsHeadersDialog(Window owner, string title, AppSettings appSettings) : base(owner, title)
        {
            AppSettings = appSettings;
            Factory = () => new();
            InitializeComponent();
        }
    }
}
