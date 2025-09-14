using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public enum FollowMode
    {
        [Display(Name = "to end")]
        ToEnd,
        [Display(Name = "loop")]
        Loop,
        [Display(Name = "teleport loop")]
        TeleportLoop,
    }
}

