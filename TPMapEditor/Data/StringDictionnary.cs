using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public static class StringDictionnary
    {
        public static Dictionary<string, string> ObjectiveTasksDictionnary { get; } = new();
        public static Dictionary<string, string> ShipNamesDictionnary { get; } = new ();
        public static Dictionary<string, string> SpeechEventsDictionnary { get; } = new();
        public static Dictionary<string, string> SpeechEventsJournalsDictionnary { get; } = new();
        public static Dictionary<string, string> SpeakerNamesDictionnary { get; } = new();
        public static Dictionary<string, string> TeamNames { get; } = new();
    }
}
