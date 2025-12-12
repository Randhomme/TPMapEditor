using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TPMapEditor.Data;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.Converter
{
    public class SelectableWorldObjectTypeConverter : IValueConverter
    {
        private bool IsSelectableWorldObjectType(CustomInfoDefinition customInfoDefinition)
        {
            return customInfoDefinition == CustomInfoDefinition.AsteroidCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.BlackHoleCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.BulletCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.DragonCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.EtheriumCurrentCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.IslandCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.MineCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.NebulaCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.NovaMortarCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.ShipCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.ShipDebrisCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.SpaceAnimalCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.StarMortarCustomInfoFactory ||
                   customInfoDefinition == CustomInfoDefinition.TorpedoCustomInfoFactory;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<WorldObjectType> worldObjectTypes)
            {
                return worldObjectTypes.Where((t) => IsSelectableWorldObjectType(t.CustomInfoDefinition));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
