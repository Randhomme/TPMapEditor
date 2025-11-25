using System;
using System.Globalization;
using System.Windows.Data;
using TPMapEditor.Enums;

namespace TPMapEditor.Converter
{
    public class EnumToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
                return EnumExtensions.GetShortName(enumValue);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
