using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Enums;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public partial class Team : CustomObservableValidator
    {
        [ObservableProperty]
        [property: Required]
        private string realName;
        [ObservableProperty]
        private Race race;
        [ObservableProperty]
        private bool raceLocked;

        public string DisplayedName {
            get
            {
                if(StringDictionnary.TeamNames.TryGetValue(RealName, out var displayName))
                {
                    return displayName;
                }
                return RealName;
            }
        }

        public Team(string realName)
        {
            this.realName = realName;
        }

        partial void OnRealNameChanged(string value)
        {
            OnPropertyChanged(nameof(DisplayedName));
        }

        public override string ToString()
        {
            return RealName;
        }
    }
}
