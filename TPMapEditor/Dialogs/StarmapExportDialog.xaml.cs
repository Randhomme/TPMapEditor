using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for StarmapExportWindow.xaml
    /// </summary>
    public partial class StarmapExportDialog : DialogWindow
    {
        public StarmapExportDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
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
                rtb.Render(MapViewbox);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fs = File.Create(sfd.FileName))
                {
                    encoder.Save(fs);
                    MessageBox.Show("Starmap image exported !", "Starmap image exported", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
