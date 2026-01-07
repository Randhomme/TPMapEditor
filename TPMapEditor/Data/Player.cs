using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Enums;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class Player : SelectableNamedMapObject, IMovableMapObject
    {
        public static string DefaultName => "NO PLAYER";

        public static Player DefaultPlayer { get; } = new(null, DefaultName);

        [ObservableProperty]
        private double x, y, z, rotation;
        [ObservableProperty]
        private Color color;
        [ObservableProperty]
        private FormationType formationTypeStart, formationType;
        [ObservableProperty]
        private Team? selectableTeam, inGameTeam;
        [ObservableProperty]
        private bool isPlayable, hasSelectableTeam, hasInGameTeam;

        public int TeamIndex { get; set; } = -1; //only used for data import

        public Player(WorldMap map) : base(map, GenerateName("Player", map.Players))
        {
            Color = Colors.Red;
            FormationTypeStart = FormationType = FormationType.Column;
            IsPlayable = true;
        }

        public Player(WorldMap map, string name) : base(map, name)
        {
            Color = Colors.Red;
            FormationTypeStart = FormationType = FormationType.Column;
            if (map != null) IsPlayable = true;
        }

        public Player(WorldMap map, string name, double x, double y, double z, double rotation, Color playerColor) : base(map, name)
        {
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
            Color = playerColor;
            FormationTypeStart = FormationType = FormationType.Column;
            IsPlayable = true;
        }

        partial void OnIsPlayableChanged(bool value)
        {
            if (value)
            {
                Map.PlayerPlayableCount++;
                if (Map.PlayerPlayableCount > 8)
                {
                    IsPlayable = false; // Ensure it doesn't stay true if count exceeds 8
                }
            }
            else
            {
                Map.PlayerPlayableCount--;
            }
        }

        partial void OnSelectableTeamChanged(Team? value)
        {
            if(value == null)
            {
                if (HasSelectableTeam)
                    HasSelectableTeam = false;
            }
            else
            {
                if (!HasSelectableTeam)
                    HasSelectableTeam = true;
            }
        }

        partial void OnHasSelectableTeamChanged(bool value)
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

        partial void OnInGameTeamChanged(Team? value)
        {
            if (value == null)
            {
                if (HasInGameTeam)
                    HasInGameTeam = false;
            }
            else if (!HasInGameTeam)
            {
                HasInGameTeam = true;
            }
        }

        partial void OnHasInGameTeamChanged(bool value)
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

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.Players)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }
    }
}
