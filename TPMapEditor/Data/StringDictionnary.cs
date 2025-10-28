using System.Collections.Generic;

namespace TPMapEditor.Data
{
    public static class StringDictionnary
    {
        public static Dictionary<string, string> MapTextItems { get; } = new();
        public static Dictionary<string, string> ObjectiveTasks { get; } = new();
        public static Dictionary<string, string> ShipNames { get; } = new ();
        public static Dictionary<string, string> SpeechEvents { get; } = new();
        public static Dictionary<string, string> SpeechEventsJournals { get; } = new();
        public static Dictionary<string, string> SpeakerNames { get; } = new();
        public static Dictionary<string, string> TeamNames { get; } = new();
        public static Dictionary<string, string> WorldNames { get; } = new();
        public static Dictionary<string, string> WorldDescriptions { get; } = new();
    }
}
