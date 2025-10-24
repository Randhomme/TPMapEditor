using System.Collections.Generic;
using TPMapEditor.Enums;

namespace TPMapEditor.Data
{
    public class Team
    {
        public string RealName { get; set; }
        public Race Race { get; set; }

        public bool RaceLocked { get; set; }

        public Team(string realName)
        {
            RealName = realName;
        }

        public override string ToString()
        {
            return RealName;
        }
    }
}
