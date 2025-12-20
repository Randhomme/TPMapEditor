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
    public class SelectableGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<Group> groupsSource)
            {
                if (groupsSource is IList<Group> groups)
                {
                    if (!groups.Contains(Group.DefaultGroup))
                        groups.Prepend(Group.DefaultGroup);
                    return groups;
                }
                return groupsSource;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
