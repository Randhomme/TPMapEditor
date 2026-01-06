using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Enums;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class SpeechEvent : NamedMapObject
    {
        [ObservableProperty]
        [property: Required]
        private string soundFileName, faceTexture, textStringID, speakerID;
        [ObservableProperty]
        private Color textColor;
        [ObservableProperty]
        private TalkingHeadLocation talkingHeadLocation;
        [ObservableProperty]
        private bool hasBeenPlayedOnce, isSecondarySpeech, openChatBar, openTalkingHead, hasText, useSoundFileLength, alwaysOpenSpeechEventBar;
        [ObservableProperty]
        private double displayTime;

        public SpeechEvent(WorldMap map, string name) : base(map, name)
        {
            soundFileName = AppSettings.DialogueFilesList.FirstOrDefault() ?? string.Empty;
            faceTexture = AppSettings.HudTexturesList.FirstOrDefault() ?? string.Empty;
            textStringID = StringDictionnary.SpeechEvents.FirstOrDefault().Key ?? string.Empty;
            speakerID = StringDictionnary.SpeakerNames.FirstOrDefault().Key ?? string.Empty;
            textColor = Colors.White;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.SpeechEvents)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
