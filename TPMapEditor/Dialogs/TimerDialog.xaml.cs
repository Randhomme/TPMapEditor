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
    /// Interaction logic for TimerDialog.xaml
    /// </summary>
    public partial class TimerDialog : DialogWindow
    {
        public WorldMap Map { get; }
        public Func<Timer> Factory { get; }

        public TimerDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            Map = map;
            Factory = () => new Data.Timer(Map, NamedElement.GenerateName("Timer", Map.Timers), false, 0);
            InitializeComponent();
        }
    }
}
