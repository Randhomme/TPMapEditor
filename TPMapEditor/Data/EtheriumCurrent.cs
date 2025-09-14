//using CommunityToolkit.Mvvm.ComponentModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TPMapEditor.Data
//{
//    public partial class EtheriumCurrent : NamedElement
//    {
//        [ObservableProperty]
//        private WorldObject? worldObject;
//        [ObservableProperty]
//        private WorldPolygon? volume;
//        public EtheriumCurrent(WorldMap map, string name) : base(map, name)
//        {
//        }

//        protected override bool IsNameTaken(string name)
//        {
//            foreach (var item in map.EtheriumCurrents)
//            {
//                if (item.Name == name && item != this)
//                    return true;
//            }
//            return false;
//        }
//    }
//}
