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
        public double BorderThickness { get; private set; }
        public double NebulaBlurRadiue { get; private set; }

        public ICollectionView Asteroids { get; }
        public ICollectionView BlackHoles { get; }
        public ICollectionView Islands { get; }
        public ICollectionView Nebulas { get; }

        public StarmapExportViewModel(WorldMap map)
        {
            this.Map = map;
            BorderThickness = 5 * Map.Size / 512;
            NebulaBlurRadiue = 15 * Map.Size / 512;
            Islands = new CollectionViewSource() { Source = map.WorldObjects }.View;
            Islands.Filter = IsWorldObjectIsland;
            BlackHoles = new CollectionViewSource() { Source = map.WorldObjects }.View;
            BlackHoles.Filter = IsWorldObjectBlackHole;
            Asteroids = new CollectionViewSource() { Source = map.WorldObjects }.View;
            Asteroids.Filter = IsWorldObjectAsteroid;
            Nebulas = new CollectionViewSource() { Source = map.Nebulas }.View;
            Nebulas.Filter = IsNebulaNebula;
        }

        private bool IsWorldObjectIsland(object o)
        {
            if(o is WorldObject worldObject)
            {
                return worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.IslandCustomInfoFactory;
            }
            return false;
        }

        private bool IsWorldObjectBlackHole(object o)
        {
            if(o is WorldObject worldObject)
            {
                return worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.BlackHoleCustomInfoFactory;
            }
            return false;
        }

        private bool IsWorldObjectAsteroid(object o)
        {
            if(o is WorldObject worldObject)
            {
                return worldObject.Type.CustomInfoDefinition == CustomInfoDefinition.AsteroidCustomInfoFactory;
            }
            return false;
        }

        private bool IsNebulaNebula(object o)
        {
            if(o is Nebula nebula)
            {
                return nebula.NebulaPointSet != WorldPointSet.DefaultWorldPointSet;
            }
            return false;
        }
    }
}
