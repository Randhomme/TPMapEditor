using System.Windows;
using CommunityToolkit.Mvvm.Input;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for FlagDialog.xaml
    /// </summary>
    public partial class FlagDialog : DialogWindow
    {
        public Flag? SelectedFlag { get; set; }
        public WorldMap Map { get; }
        public FlagDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddFlag()
        {
            Map.Flags.Add(new Flag(NamedElement.GenerateName("Flag", Map.Flags), Map));
        }

        private void RemoveFlag_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFlag != null)
                Map.Flags.Remove(SelectedFlag);
        }
    }
}
