using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TPMapEditor.Data;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.ViewModel
{
    public partial class StarmapExportViewModel : ObservableObject
    {
        public WorldMap Map { get; }

        public double NegativeWorldBuffer { get => -Map.WorldBuffer; }

        public ICollectionView StarmapWorldObjects { get; }
        public ICollectionView BlackHoles { get; }

        public StarmapExportViewModel(WorldMap map)
        {
            this.Map = map;
            StarmapWorldObjects = new CollectionViewSource() { Source = map.WorldObjects }.View;
            StarmapWorldObjects.Filter = IsWorldObjectDisplayedOnStarmap;
            BlackHoles = new CollectionViewSource() { Source = map.WorldObjects }.View;
            BlackHoles.Filter = IsWorldObjectBlackHole;
        }

        /// <summary>
        /// Filters world objects to get only the starmap world objects (without blackholes, we want them in an other collection)
        /// </summary>
        private bool IsWorldObjectDisplayedOnStarmap(object o)
        {
            if(o is WorldObject worldObject)
            {
                return worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.AsteroidCustomInfoFactory
                    || worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.IslandCustomInfoFactory;
            }
            return false;
        }

        /// <summary>
        /// Filters world objects to get only the black holes, we want them in an other collection
        /// </summary>
        private bool IsWorldObjectBlackHole(object o)
        {
            if(o is WorldObject worldObject)
            {
                return worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.BlackHoleCustomInfoFactory;
            }
            return false;
        }
    }
}
