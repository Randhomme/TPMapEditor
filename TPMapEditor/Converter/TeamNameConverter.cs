using System;
using System.Globalization;
using System.Windows.Data;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TeamNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StringDictionnary.TeamNames.TryGetValue(value.ToString(), out var displayedName) ? displayedName : value;
        }

        //We'll never get here because we don't go from the displayed name to the real name.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
