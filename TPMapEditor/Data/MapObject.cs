using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Interfaces;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public abstract partial class MapObject : CustomObservableValidator, IMapObject
    {
        public WorldMap Map { get; }
        protected MapObject(WorldMap map)
        {
            Map = map;
        }
    }
}
