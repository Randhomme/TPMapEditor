using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Windows;
using TPMapEditor.Settings;
using WF = System.Windows.Forms;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for AppSettingsDialog.xaml
    /// </summary>
    public partial class AppSettingsDialog : DialogWindow
    {
        public AppSettings AppSettings { get; }
        public AppSettingsDialog(Window owner, string title, AppSettings appSettings) : base(owner, title)
        {
            AppSettings = appSettings;
            InitializeComponent();
        }

        [RelayCommand]
        private void OnBrowseTpGamePath()
        {
            var ofd = new WF.FolderBrowserDialog()
            {
                Description = "Select the TPGame folder",
                ShowNewFolderButton = false,
                SelectedPath = AppSettings.TpGamePath
            };
            if (ofd.ShowDialog() == WF.DialogResult.OK)
            {
                AppSettings.TpGamePath = ofd.SelectedPath;
            }
        }

        [RelayCommand]
        private void OnBrowseBgImagePath()
        {
            var ofd = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "Select an image for the map editor background",
                Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff;*.ico|" +
                 "JPEG|*.jpg;*.jpeg|" +
                 "PNG|*.png|" +
                 "BMP|*.bmp|" +
                 "GIF|*.gif|" +
                 "TIFF|*.tif;*.tiff|" +
                 "ICO|*.ico|" +
                 "All files|*.*"
            };
            if(ofd.ShowDialog() == true)
            {
                AppSettings.BgImagePath = ofd.FileName;
            }
        }

        private void DialogWindow_Closed(object sender, EventArgs e)
        {
                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var gshd = new GameStringsHeadersDialog(this, "Game strings headers", AppSettings);
            gshd.ShowDialog();
            new ProgressDialog(this, "Reload string").RunActionSameThread((progress, logs) =>
            {
                progress.Report("Reloading strings ...");
                AppSettings.ReloadStringsDictionnaries(progress, logs);
                progress.Report("Reloading complete");
            }, true);
        }

        private void DialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
