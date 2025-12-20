using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TeamsDialog.xaml
    /// </summary>
    public partial class TeamsDialog : DialogWindow
    {
        public WorldMap Map { get; }
        public Func<Team> Factory { get; }

        public TeamsDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            Map = map;
            Factory = () => new(StringDictionnary.TeamNames.Keys.FirstOrDefault());
            InitializeComponent();
        }
    }
}
