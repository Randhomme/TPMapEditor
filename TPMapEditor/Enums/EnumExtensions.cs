using System;
using System.ComponentModel.DataAnnotations;

namespace TPMapEditor.Enums
{
    public static class EnumExtensions
    {
        public static string GetName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute?.Name ?? value.ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute?.Description ?? value.ToString();
        }

    }
}
