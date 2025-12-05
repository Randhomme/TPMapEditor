using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class WotTypesToCrewTypesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<WorldObjectType> wotGridItems)
            {
                return wotGridItems.Where((t) => t.CustomInfoDefinition == Enums.WorldObjectDefinition.CustomInfoDefinition.CrewCustomInfoFactory);

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
