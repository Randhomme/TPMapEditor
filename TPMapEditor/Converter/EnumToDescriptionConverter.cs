using System;
using System.Globalization;
using System.Windows.Data;
using TPMapEditor.Enums;

namespace TPMapEditor.Converter
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
                return EnumExtensions.GetDescription(enumValue);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}