using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for GroupDialog.xaml
    /// </summary>
    [ObservableObject]
    public partial class GroupDialog : DialogWindow
    {
        [ObservableProperty]
        private Group? selectedGroup;
        [ObservableProperty]
        private WorldObject? selectedWot;
        public WorldMap Map { get; }
        public GroupDialog(Window owner, WorldMap map) : base(owner)
        {
            Map = map;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnAddGroup()
        {
            Map.Groups.Add(new Group(NamedElement.GenerateName("Group", Map.Groups), Map));
        }

        private void RemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroup != null)
                Map.Groups.Remove(SelectedGroup);
        }

        private void EditGroupColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroup != null)
            {
                var cp = new ColorPicker(this, SelectedGroup.Color, 255);
                if (cp.ShowDialog() == true)
                {
                    SelectedGroup.Color = cp.NewColor;
                }
            }
        }

        private void RemoveWot_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWot != null)
                SelectedWot.Group = null;
        }
    }
}
