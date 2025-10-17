using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public enum Equivalence
    {
        [Display(Name = "Equal To")]
        EqualTo,
        [Display(Name = "Greater Than")]
        GreaterThan,
        [Display(Name = "Less Than")]
        LessThan
    }
}
