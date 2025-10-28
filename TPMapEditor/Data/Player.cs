using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Media;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public partial class Player : NamedElement
    {
        [ObservableProperty]
        private double x, y, z, rotation;
        [ObservableProperty]
        private Color color;
        [ObservableProperty]
        private FormationType formationTypeStart, formationType;
        [ObservableProperty]
        private Team? team;
        [ObservableProperty]
        private bool isPlayable;

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

        public Player(string name, WorldMap map, double x, double y, double z, double rotation, Color playerColor) : base(map, name)
        {
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
            Color = playerColor;
            FormationTypeStart = FormationType = FormationType.Column;
            IsPlayable = true;
        }
    }
}
