using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TPMapEditor.Data;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.Converter
{
    internal class WotTypesToParameterType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<WorldObject> wots)
            {
                if(parameter is CustomInfoDefinition cid)
                {
                    return wots.Where((wot) => wot.Type.CustomInfoDefinition == cid);
                }
                else if(parameter is IEnumerable<CustomInfoDefinition> cids)
                {
                    return wots.Where((wot) => CheckWotCustomInfo(wot.Type, cids));
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool CheckWotCustomInfo(WorldObjectType wot, IEnumerable<CustomInfoDefinition> customInfos)
        {
            foreach(var customInfo in customInfos)
            {
                if (wot.CustomInfoDefinition == customInfo)
                    return true;
            }
            return false;
        }
    }
}
