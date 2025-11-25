using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool canClose, progressBarIndeterminate = true;

        [ObservableProperty]
        private string logs, currentOperation = "Working...";

        public IProgress<string> Progress { get; }

        public IProgress<string> ProgressOperation { get; }

        public ProgressDialog(Window owner) : base(owner)
        {
            logs = "";
            Progress = new Progress<string>(ProgressReport);
            ProgressOperation = new Progress<string>(ProgressOperationReport);
            InitializeComponent();
        }

        [RelayCommand]
        private void OnClose()
        {
            this.DialogResult = true;
        }

        private void ProgressReport(string s)
        {
            Logs += s + Environment.NewLine;
        }

        private void ProgressOperationReport(string s)
        {
            CurrentOperation = s;
        }

        partial void OnCanCloseChanged(bool value)
        {
            if (value)
                ProgressBarIndeterminate = false;
        }
    }
}
