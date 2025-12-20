using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
