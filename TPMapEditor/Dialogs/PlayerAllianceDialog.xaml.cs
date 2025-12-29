using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PlayerAllianceWindow.xaml
    /// </summary>
    public partial class PlayerAllianceDialog : DialogWindow
    {
        public WorldMap Map { get; }
        public Func<PlayerAlliance> Factory { get; }
        public PlayerAllianceDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            this.Map = map;
            this.Factory = () => new(Player.DefaultPlayer, Player.DefaultPlayer);
            InitializeComponent();
        }
    }
}
