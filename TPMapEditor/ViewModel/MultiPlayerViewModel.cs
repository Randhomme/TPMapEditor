using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Enums;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class MultiPlayerViewModel : MultiRotatableMapObjectViewModel<Player>, IColoredMapObject
    {
        [ObservableProperty]
        private Color color;
        [ObservableProperty]
        private FormationType formationTypeStart, formationType;
        [ObservableProperty]
        private Team? selectableTeam, inGameTeam;
        [ObservableProperty]
        private bool isPlayable, hasSelectableTeam, hasInGameTeam;

        private Player? selectedPlayer;

        public WorldMap Map { get; }
        public string Name
        {
            get => selectedPlayer?.Name ?? string.Empty;
            set
            {
                if (selectedPlayer != null) selectedPlayer.Name = value;
            }
        }
        public bool ShowName { get => Count == 1; }



        public MultiPlayerViewModel(IEnumerable<Player> selectedMapObjects, IUndoManagerService undoManagerService, WorldMap map) : base(selectedMapObjects, undoManagerService)
        {
            Map = map;
        }

        protected override void UpdateFromMapObject_Internal(Player mapObject)
        {
            selectedPlayer = mapObject;
            base.UpdateFromMapObject_Internal(mapObject);
            Color = mapObject.Color;
            FormationTypeStart = mapObject.FormationTypeStart;
            FormationType = mapObject.FormationType;
            SelectableTeam = mapObject.SelectableTeam;
            InGameTeam = mapObject.InGameTeam;
            IsPlayable = mapObject.IsPlayable;
            HasSelectableTeam = mapObject.HasSelectableTeam;
            HasInGameTeam = mapObject.HasInGameTeam;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ShowName));
        }

        partial void OnColorChanged(Color value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.Color = value;
                }
            }
        }

        partial void OnFormationTypeChanged(FormationType value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.FormationType = value;
                }
            }
        }

        partial void OnFormationTypeStartChanged(FormationType value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.FormationTypeStart = value;
                }
            }
        }

        partial void OnIsPlayableChanged(bool value)
        {
            if (UseUpdateCommands)
            {
                foreach (var item in selectedMapObjects)
                {
                    item.IsPlayable = value;
                }
            }
        }

        partial void OnSelectableTeamChanged(Team? value)
        {
            if (UseUpdateCommands)
            {
                if (value != null)
                {
                    HasSelectableTeam = true;
                }
                else
                {
                    HasSelectableTeam = false;
                }
                foreach (var item in selectedMapObjects)
                {
                    item.SelectableTeam = value;
                }
            }
        }

        partial void OnHasSelectableTeamChanged(bool value)
        {
            if (UseUpdateCommands)
            {
                if (value)
                {
                    SelectableTeam ??= Map.SelectableTeams.FirstOrDefault();
                }
                else
                {
                    SelectableTeam = null;
                }
            }
        }

        partial void OnInGameTeamChanged(Team? value)
        {
            if (UseUpdateCommands)
            {
                if (value != null)
                {
                    HasInGameTeam = true;
                }
                else
                {
                    HasInGameTeam = false;
                }
                foreach (var item in selectedMapObjects)
                {
                    item.InGameTeam = value;
                }
            }
        }

        partial void OnHasInGameTeamChanged(bool value)
        {
            if (UseUpdateCommands)
            {
                if (value)
                {
                    InGameTeam ??= Map.InGameTeams.FirstOrDefault();
                }
                else
                {
                    InGameTeam = null;
                }
            }
        }
    }
}
