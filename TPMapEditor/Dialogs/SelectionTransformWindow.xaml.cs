using System.Windows;
using TPMapEditor.ViewModel.SelectionTransform;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectionTransformWindow.xaml
    /// </summary>
    public partial class SelectionTransformWindow : Window
    {
        private readonly SelectionTransformBaseViewModel vm;
        public SelectionTransformWindow(Window owner, string title, SelectionTransformBaseViewModel vm)
        {
            this.Owner = owner;
            this.Title = title;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.DataContext = this.vm = vm;
            InitializeComponent();
            this.Closed += (s, e) =>
            {
                if (vm.ShouldCommitCommand)
                    vm.CommitCommand();
                else
                    vm.CancelCommand();
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            vm.ShouldCommitCommand = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            vm.ShouldCommitCommand = false;
            this.Close();
        }
    }
}
