using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace TPMapEditor.Dialogs
{
    [ObservableObject]
    public partial class DialogWindow : Window
    {
        public DialogWindow(Window owner)
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        public bool? ShowDialog(bool showInTaskBar = false)
        {
            ShowInTaskbar = showInTaskBar;
            return base.ShowDialog();
        }

        protected void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        protected void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
