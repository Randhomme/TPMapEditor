using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Enums;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class SpeechEvent : NamedElement
    {
        [ObservableProperty]
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
            faceTexture = AppSettings.FaceTexturesList.FirstOrDefault() ?? string.Empty;
            textStringID = StringDictionnary.SpeechEvents.FirstOrDefault().Key ?? string.Empty;
            speakerID = StringDictionnary.SpeakerNames.FirstOrDefault().Key ?? string.Empty;
            textColor = Colors.White;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.SpeechEvents)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
