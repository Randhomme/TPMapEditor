using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TPMapEditor.Converter
{
    public class CenteringMultiConverter : IMultiValueConverter
    {
        // values[0] = X or Y
        // values[1] = actualSize (ActualWidth or ActualHeight)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return 0.0;

            if (!TryToDouble(values[0], out double coord))
                coord = 0.0;

            if (!TryToDouble(values[1], out double size))
                size = 0.0;

            // coord - halfsize
            return coord - (size / 2.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool TryToDouble(object o, out double d)
        {
            d = 0;
            if (o == null) return false;
            if (o is double dd) { d = dd; return true; }
            if (double.TryParse(o.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out dd)) { d = dd; return true; }
            return false;
        }
    }
}
