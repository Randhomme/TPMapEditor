using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.Data
{
    public partial class WorldObjectType : NamedObject
    {
        public static ObservableCollection<WorldObjectType> WotTypes { get; } = new ObservableCollection<WorldObjectType>();
        public static BitmapImage WotPlaceholder { get; } = new BitmapImage(new Uri("/Images/WotPlaceholder.png", UriKind.Relative));

        [ObservableProperty]
        private BitmapImage image = WotPlaceholder;
        [ObservableProperty]
        private Point pivot = new(0.5, 0.5);
        [ObservableProperty]
        private CustomInfoDefinition customInfoDefinition;

        public WorldObjectType(string name) : base(name)
        {
        }

        public override string ToString()
        {
            return Name ?? "WorldObject";
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

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in WotTypes)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }

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
