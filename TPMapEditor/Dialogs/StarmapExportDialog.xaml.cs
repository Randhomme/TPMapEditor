using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.ViewModel;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for StarmapExportWindow.xaml
    /// </summary>
    public partial class StarmapExportDialog : Window
    {
        public StarmapExportDialog(Window owner, string title)
        {
            InitializeComponent();
            Title = title;
            Loaded += async (s, e) =>
            {
                var vm = (StarmapExportViewModel)DataContext;
                await vm.ProcessIslandsAndAsteroidsImages();
                vm.IsReady = true;
            };
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                DefaultExt = ".png",
                Filter = "PNG image (.png)|*.png",
                Title = "Export the starmap image",
            };
            if (sfd.ShowDialog(this) == true)
            {
                var rtb = new RenderTargetBitmap(512, 512, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(MapGrid);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fs = File.Create(sfd.FileName))
                {
                    encoder.Save(fs);
                }
                MessageBox.Show("Starmap image exported !", "Starmap image exported", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (StarmapExportViewModel)DataContext;
            vm.Close();
        }
    }
}
