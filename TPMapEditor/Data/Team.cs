using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public partial class Team : ObservableObject
    {
        [ObservableProperty]
        private string realName;
        [ObservableProperty]
        private Race race;
        [ObservableProperty]
        private bool raceLocked;

        public Team(string realName)
        {
            this.realName = realName;
        }
    }
}
