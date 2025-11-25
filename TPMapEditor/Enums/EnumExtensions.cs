using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        public static string GetShortName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute?.ShortName ?? attribute?.Name ?? value.ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return attribute?.Description ?? value.ToString();
        }

        public static bool TryGetValueFromDisplayName<TEnum>(string displayName, out TEnum value) where TEnum : struct, Enum
        {
            if (displayName is null) throw new ArgumentNullException(nameof(displayName));

            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                var candidate = attr?.Name ?? field.Name;

                if (string.Equals(candidate, displayName, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(field.Name, displayName, StringComparison.OrdinalIgnoreCase))
                {
                    var raw = field.GetValue(null);
                    value = (TEnum)Enum.ToObject(typeof(TEnum), raw);
                    return true;
                }
            }

            value = default;
            return false;
        }

    }
}
