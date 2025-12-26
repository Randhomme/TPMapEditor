using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.Data
{
    /// <summary>
    /// A class representing a world object in the selection grid.
    /// </summary>
    public class WorldObjectType
    {
        public static ObservableCollection<WorldObjectType> WotTypes { get; } = new ObservableCollection<WorldObjectType>();
        public BitmapImage? Image { get; set; }
        public string Type { get; set; } = string.Empty;
        public Point Pivot { get; set; } = new Point(0.5, 0.5);
        public CustomInfoDefinition CustomInfoDefinition { get; set; }

        public override string ToString()
        {
            return Type ?? "WorldObject";
        }

        public static bool IsSelectableWorldObjectType(object o)
        {
            if (o is WorldObjectType wot)
            {
                return wot.CustomInfoDefinition == CustomInfoDefinition.AsteroidCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.BlackHoleCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.BulletCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.DragonCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.EtheriumCurrentCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.IslandCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.MineCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.NebulaCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.NovaMortarCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.ShipCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.SpaceAnimalCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.StarMortarCustomInfoFactory ||
                   wot.CustomInfoDefinition == CustomInfoDefinition.TorpedoCustomInfoFactory;
            }
            return false;

        }
    }

    /*
    /// <summary>
    /// Not really a data class, only used to get the list from WorldObjects.xml
    /// </summary>
    [XmlRoot("Objects")]
    public class WorldObjectTypeXmlCollection
    {
        [XmlElement("Object")]
        public List<WorldObjectTypeXml> Items { get; set; } = new();
    }

    /// <summary>
    /// Not really a data class, only used to get the pivot from a WorldObjectType
    /// </summary>
    public class WorldObjectTypeXml
    {
        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("PivotX")]
        public double PivotX { get; set; }

        [XmlElement("PivotY")]
        public double PivotY { get; set; }
    }
    */
    /// <summary>
    /// Not really a data class, only used to get the pivot from a WorldObjectType
    /// </summary>
    [XmlRoot("Object")]
    public class WorldObjectTypeXml
    {
        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("PivotX")]
        public double PivotX { get; set; }

        [XmlElement("PivotY")]
        public double PivotY { get; set; }
    }

}
