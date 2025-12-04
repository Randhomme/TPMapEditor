using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Media;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public partial class Player : NamedElement
    {
        public static string DefaultName => "NO PLAYER";

        [ObservableProperty]
        private double x, y, z, rotation;
        [ObservableProperty]
        private Color color;
        [ObservableProperty]
        private FormationType formationTypeStart, formationType;
        [ObservableProperty]
        private Team? selectableTeam, inGameTeam;
        [ObservableProperty]
        private bool isPlayable, isSelected, isLastSelected;

        public int TeamIndex { get; set; } //only used for data import

        public Player(WorldMap map, string name, double x, double y, double z, double rotation, Color playerColor) : base(map, name)
        {
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
            Color = playerColor;
            FormationTypeStart = FormationType = FormationType.Column;
            IsPlayable = true;
            TeamIndex = -1;
        }

        partial void OnIsPlayableChanged(bool value)
        {
            if (value)
            {
                map.PlayerPlayableCount++;
                if (map.PlayerPlayableCount > 8)
                {
                    IsPlayable = false; // Ensure it doesn't stay true if count exceeds 8
                }
            }
            else
            {
                map.PlayerPlayableCount--;
            }
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.Players)
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
