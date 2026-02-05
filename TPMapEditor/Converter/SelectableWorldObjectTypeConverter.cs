using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class SelectableWorldObjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<WorldObjectType> worldObjectTypes)
            {
                return worldObjectTypes.Where((wot) => WorldObjectType.IsSelectableWorldObjectType(wot));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
