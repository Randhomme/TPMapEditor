using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    /// <summary>
    /// Class representing a unit in world rules
    /// </summary>
    public partial class ShipUnit : NamedElement
    {
        public static string DefaultName => "HUMAN CONTROLLED COMMAND SHIP";

        [ObservableProperty]
        private WorldObject? worldObject;

        public ShipUnit(WorldMap map, string name, WorldObject? worldObject = null) : base(map, name)
        {
            this.worldObject = worldObject ?? map.WorldObjects.FirstOrDefault();
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
