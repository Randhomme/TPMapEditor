using CommunityToolkit.Mvvm.Input;
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
