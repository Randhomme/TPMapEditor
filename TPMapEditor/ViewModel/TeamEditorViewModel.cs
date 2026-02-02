using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TPMapEditor.Data;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class TeamEditorViewModel : ObservableObject
    {
        private bool canPasteSelectableTeams = false;
        private bool canPasteInGameTeams = false;
        private readonly ICopyPasteService copyPasteService;
        private readonly ObservableCollection<Team> selectedSelectableTeams = new();
        private readonly ObservableCollection<Team> selectedInGameTeams = new();
        public ICollection<Team> SelectableTeams { get; }
        public ICollection<Team> InGameTeams { get; }
        public ICollection<Team> SelectedSelectableTeams { get => selectedSelectableTeams; }
        public ICollection<Team> SelectedInGameTeams { get => selectedInGameTeams; }
        public Func<object> Factory { get; }
        public bool GridOnlyMode { get; }

        public TeamEditorViewModel(ICollection<Team> selectableTeams, ICollection<Team> inGameTeams, Func<object> factory, ICopyPasteService copyPasteService, bool gridOnlyMode = true)
        {
            this.SelectableTeams = selectableTeams;
            this.InGameTeams = inGameTeams;
            this.Factory = factory;
            this.copyPasteService = copyPasteService;
            this.GridOnlyMode = gridOnlyMode;
            selectedSelectableTeams.CollectionChanged += (s, e) =>
            {
                CopySelectableTeamsCommand.NotifyCanExecuteChanged();
            };
            selectedInGameTeams.CollectionChanged += (s, e) =>
            {
                CopyInGameTeamsCommand.NotifyCanExecuteChanged();
            };
            copyPasteService.ClearClipboard();
        }


        [RelayCommand(CanExecute = nameof(CanCopySelectableTeams))]
        private void OnCopySelectableTeams()
        {
            copyPasteService.Copy(SelectedSelectableTeams);
            canPasteSelectableTeams = true;
            PasteSelectableTeamsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPasteSelectableTeams))]
        private void OnPasteSelectableTeams()
        {
            var pastedItems = copyPasteService.Paste<Team>();
            foreach (var item in pastedItems)
            {
                SelectableTeams.Add(item);
            }
        }

        [RelayCommand(CanExecute = nameof(CanCopyInGameTeams))]
        private void OnCopyInGameTeams()
        {
            copyPasteService.Copy(SelectedInGameTeams);
            canPasteInGameTeams = true;
            PasteInGameTeamsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPasteInGameTeams))]
        private void OnPasteInGameTeams()
        {
            var pastedItems = copyPasteService.Paste<Team>();
            foreach (var item in pastedItems)
            {
                InGameTeams.Add(item);
            }
        }

        private bool CanCopySelectableTeams() => SelectedSelectableTeams.Count > 0;

        private bool CanPasteSelectableTeams() => canPasteSelectableTeams;

        private bool CanCopyInGameTeams() => SelectedInGameTeams.Count > 0;

        private bool CanPasteInGameTeams() => canPasteInGameTeams;
    }
}
