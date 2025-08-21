using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TPMapEditor.Data;

namespace TPMapEditor.Converter
{
    public class YConverter : IValueConverter
    {
        private FrameworkElement element;
        public YConverter(FrameworkElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// Convert from object Y position on the map to Canvas.Top.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
            {
                return dValue - element.ActualHeight / 2;
            }
            return value;
        }

        /// <summary>
        /// Convert from Canvas.Top to object Y position on the map.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
            {
                return dValue + element.ActualHeight / 2;
            }
            return value;
        }
    }
}
