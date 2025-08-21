using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public partial class SpeechEvent : NamedElement
    {
        public static IList<string> DialogueFilesList { get; } = new List<string>();
        public static IList<string> FaceTexturesList { get; } = new List<string>();
        public static Dictionary<string, string> SpeechEventDictionnary { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> SpeakerNamesDictionnary { get; } = new Dictionary<string, string>();
        [ObservableProperty]
        private string soundFileName, faceTexture, textStringID, speakerID;
        [ObservableProperty]
        private Color textColor;
        [ObservableProperty]
        private TalkingHeadLocation talkingHeadLocation;
        [ObservableProperty]
        private bool hasBeenPlayedOnce, isSecondarySpeech, openChatBar, openTalkingHead, hasText, useSoundFileLength, alwaysOpenSpeechEventBar;
        [ObservableProperty]
        private float displayTime;

        public SpeechEvent(WorldMap map, string name) : base(map, name)
        {
            soundFileName = DialogueFilesList.FirstOrDefault() ?? string.Empty;
            faceTexture = FaceTexturesList.FirstOrDefault() ?? string.Empty;
            textStringID = SpeechEventDictionnary.FirstOrDefault().Key ?? string.Empty;
            speakerID = SpeakerNamesDictionnary.FirstOrDefault().Key ?? string.Empty;
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
