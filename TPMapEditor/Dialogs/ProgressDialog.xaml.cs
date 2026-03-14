using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgessDialog.xaml
    /// </summary>
    public partial class ProgressDialog : DialogWindow
    {
        [ObservableProperty]
        private bool canClose, autoClose, notifyOnAutoClose = true, progressBarIndeterminate = true;

        [ObservableProperty]
        private string logs, currentOperation = "Working...";

        public IProgress<string> Progress { get; }

        public IProgress<string> ProgressLogs { get; }

        public ProgressDialog(Window owner, string title) : base(owner, title)
        {
            logs = string.Empty;
            Progress = new Progress<string>(ProgressReport);
            ProgressLogs = new Progress<string>(ProgressLogsReport);
            InitializeComponent();
        }

        public void RunAction(Action<IProgress<string>, IProgress<string>> action, bool autoClose = false, bool notifyOnAutoClose = true)
        {
            Logs = string.Empty;
            this.AutoClose = autoClose;
            this.NotifyOnAutoClose = notifyOnAutoClose;
            Task.Run(() =>
            {
                action?.Invoke(this.Progress, this.ProgressLogs);
                this.Dispatcher.Invoke(() => { CanClose = true; }, System.Windows.Threading.DispatcherPriority.SystemIdle);
            });
            this.ShowDialog();
        }

        public void RunActionSameThread(Action<IProgress<string>, IProgress<string>> action, bool autoClose = false, bool notifyOnAutoClose = true)
        {
            Logs = string.Empty;
            this.AutoClose = autoClose;
            this.NotifyOnAutoClose = notifyOnAutoClose;
            this.Dispatcher.InvokeAsync(() =>
            {
                action?.Invoke(this.Progress, this.ProgressLogs);
                this.Dispatcher.Invoke(() => { CanClose = true; }, System.Windows.Threading.DispatcherPriority.SystemIdle);
            }, System.Windows.Threading.DispatcherPriority.SystemIdle);
            this.ShowDialog();
        }

        [RelayCommand]
        private void OnClose()
        {
            this.DialogResult = true;
        }

        private void ProgressReport(string s)
        {
            CurrentOperation = s;
        }

        private void ProgressLogsReport(string s)
        {
            Logs += s + Environment.NewLine;
        }

        partial void OnCanCloseChanged(bool value)
        {
            if (value)
            {
                ProgressBarIndeterminate = false;
                if (AutoClose && string.IsNullOrEmpty(Logs))
                {
                    this.Dispatcher.Invoke(() => this.Close());
                    if (NotifyOnAutoClose)
                        MessageBox.Show($"{CurrentOperation}", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
