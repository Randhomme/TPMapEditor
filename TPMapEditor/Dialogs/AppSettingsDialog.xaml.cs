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
                Description = "Select the TPGame folder.",
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
            AppSettings.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var gshd = new GameStringsHeadersDialog(this, "Game strings headers", AppSettings);
            gshd.ShowDialog();
            AppSettings.UpdateStringsDictionnaries();
        }
    }
}
