using System.Collections.Generic;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public partial class Team
    {
        public static Dictionary<string, string> TeamNames { get; } = new Dictionary<string, string>();
        public string? RealName { get; set; }
        public Race Race { get; set; }
    }
}
