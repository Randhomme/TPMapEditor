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
    public class SelectableShipUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ShipUnit> shipUnitsSource)
            {
                if (shipUnitsSource is IList<ShipUnit> shipUnits)
                {
                    if (!shipUnits.Contains(ShipUnit.DefaultShipUnit))
                        shipUnits.Insert(0, ShipUnit.DefaultShipUnit);
                    return shipUnits;
                }
                return shipUnitsSource;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
