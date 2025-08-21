using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.Data;
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
        public AppSettingsDialog(Window owner, AppSettings appSettings) : base(owner)
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
            var gshd = new GameStringsHeadersDialog(this, AppSettings);
            gshd.ShowDialog();
            AppSettings.UpdateStringsDictionnaries();
        }
    }
}
