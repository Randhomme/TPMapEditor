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

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgessDialog.xaml
    /// </summary>
    public partial class ProgressDialog : DialogWindow
    {
        [ObservableProperty]
        private string logText;

        [ObservableProperty]
        private bool canClose, progressBarIndeterminate = true;

        public IProgress<string> Progress { get; }

        public ProgressDialog(Window owner) : base(owner)
        {
            logText = "";
            Progress = new Progress<string>(ProgressReport);
            InitializeComponent();
        }

        [RelayCommand]
        private void OnClose()
        {
            this.DialogResult = true;
        }

        private void ProgressReport(string s)
        {
            LogText += s + Environment.NewLine;
        }

        partial void OnCanCloseChanged(bool value)
        {
            if (value)
                ProgressBarIndeterminate = false;
        }
    }
}
