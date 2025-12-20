using System;
using System.Collections.Generic;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for FlagDialog.xaml
    /// </summary>
    public partial class FlagDialog : DialogWindow
    {
        public WorldMap Map { get; }
        public Func<Flag> Factory { get; }

        public FlagDialog(Window owner, string title, WorldMap map) : base(owner, title)
        {
            Map = map;
            Factory = () => new(map);
            InitializeComponent();
        }
    }
}
