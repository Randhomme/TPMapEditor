using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Data;
using TPMapEditor.Interfaces;
using TPMapEditor.Utils;

namespace TPMapEditor.Interfaces.Implementations
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
