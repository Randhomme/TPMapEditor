using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Enums.WorldObjectDefinition;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    /// <summary>
    /// Class representing a unit in world rules
    /// </summary>
    public partial class ShipUnit : NamedMapObject
    {
        public static string DefaultName => "HUMAN CONTROLLED COMMAND SHIP";

        public static ShipUnit DefaultShipUnit { get; } = new(null, DefaultName);

        [ObservableProperty]
        [property: Required]
        private WorldObject? worldObject;

        public ShipUnit(WorldMap map, string name, WorldObject? worldObject = null) : base(map, name)
        {
            this.worldObject = worldObject ?? map?.WorldObjects.FirstOrDefault((w) => w.Type.CustomInfoDefinition == CustomInfoDefinition.ShipCustomInfoFactory);
        }

        protected override bool IsNameTaken(string name)
        {
            //if(WorldObject?.Group != null)
            //{
            //    foreach (var item in WorldObject.Group.ShipUnits)
            //    {
            //        if (item.Name == name && item != this)
            //            return true;
            //    }
            //}
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }
    }
}
