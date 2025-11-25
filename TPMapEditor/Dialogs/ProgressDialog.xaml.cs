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

        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        public IProgress<string> Progress { get; }

        public ProgressDialog(Window owner) : base(owner)
        {
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
            Logs.Add(s);
            //LogsListBox.ScrollIntoView(s);
        }

        partial void OnCanCloseChanged(bool value)
        {
            if (value)
                ProgressBarIndeterminate = false;
        }
    }
}
