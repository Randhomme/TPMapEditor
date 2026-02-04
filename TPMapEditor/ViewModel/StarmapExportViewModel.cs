using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TPMapEditor.Data;
using TPMapEditor.Enums.WorldObjectDefinition;
using TPMapEditor.Utils;

namespace TPMapEditor.ViewModel
{
    public partial class StarmapExportViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isReady = false;
        private readonly Brush islandBrush;
        private readonly Brush asteroidBrush;
        private Color outlineColor;

        public WorldMap Map { get; }
        public double NegativeWorldBuffer { get => -Map.WorldBuffer; }
        public double BorderThickness { get; } = 2;
        public double TranslateTransformIsland { get; } = -0.5;
        public double ScaleDownTransformIsland { get; } = 0.001;
        public double NegativeBorderThickness { get; }
        public double EtheriumCurrentThickness { get; } = 9;
        public double EtheriumCurrentShadowThickness { get; } = 6;
        public double NebulaBlurRadius { get; } = 15;
        public double IslandShadowBlurRadius { get; } = 40;
        public ICollectionView Asteroids { get; }
        public ICollectionView BlackHoles { get; }
        public ICollectionView Islands { get; }
        public ICollectionView Nebulas { get; }
        public ICollectionView EtheriumCurrents { get; }

        private readonly SemaphoreSlim _semaphore = new(4); // CPU limit

        public StarmapExportViewModel(WorldMap map)
        {
            islandBrush = new SolidColorBrush(Colors.Black);
            asteroidBrush = new SolidColorBrush(Colors.Black);
            outlineColor = Colors.White;
            this.Map = map;
            BorderThickness *= (Map.Size - Map.WorldBuffer) / 512.0;
            NegativeBorderThickness = -BorderThickness;
            NebulaBlurRadius *= (Map.Size - Map.WorldBuffer) / 512;
            EtheriumCurrentThickness *= (Map.Size - Map.WorldBuffer) / 512;
            EtheriumCurrentShadowThickness *= (Map.Size - Map.WorldBuffer) / 512;
            IslandShadowBlurRadius *= (Map.Size - Map.WorldBuffer) / 512;
            TranslateTransformIsland *= (Map.Size - Map.WorldBuffer) / 512;
            ScaleDownTransformIsland = 1 - ScaleDownTransformIsland * (Map.Size - Map.WorldBuffer) / 512;
            IList<StarmapWorldObjectViewModel> worldObjectViewModels = new List<StarmapWorldObjectViewModel>(map.WorldObjects.Select((w) => new StarmapWorldObjectViewModel(w)));
            Islands = new CollectionViewSource() { Source = worldObjectViewModels }.View;
            Islands.Filter = IsWorldObjectIsland;
            BlackHoles = new CollectionViewSource() { Source = worldObjectViewModels }.View;
            BlackHoles.Filter = IsWorldObjectBlackHole;
            Asteroids = new CollectionViewSource() { Source = worldObjectViewModels }.View;
            Asteroids.Filter = IsWorldObjectAsteroid;
            Nebulas = new CollectionViewSource() { Source = map.Nebulas }.View;
            Nebulas.Filter = IsNebulaNebula;
            EtheriumCurrents = new CollectionViewSource() { Source = map.EtheriumCurrents }.View;
        }

        private bool IsWorldObjectIsland(object o)
        {
            if(o is StarmapWorldObjectViewModel worldObject)
            {
                return worldObject.CustomInfo == CustomInfoDefinition.IslandCustomInfoFactory;
            }
            return false;
        }

        private bool IsWorldObjectBlackHole(object o)
        {
            if(o is StarmapWorldObjectViewModel worldObject)
            {
                return worldObject.CustomInfo == CustomInfoDefinition.BlackHoleCustomInfoFactory;
            }
            return false;
        }

        private bool IsWorldObjectAsteroid(object o)
        {
            if(o is StarmapWorldObjectViewModel worldObject)
            {
                return worldObject.CustomInfo == CustomInfoDefinition.AsteroidCustomInfoFactory;
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

        public async Task ProcessIslandsAndAsteroidsImages()
        {
            var islands = Islands.Cast<StarmapWorldObjectViewModel>().ToList();
            var asteroids = Asteroids.Cast<StarmapWorldObjectViewModel>().ToList();
            var islandsTasks = islands.Select(ProcessIslandsAsync);
            var asteroidsTasks = asteroids.Select(ProcessAsteroidsAsync);
            await Task.WhenAll(islandsTasks);
            await Task.WhenAll(asteroidsTasks);
        }

        private async Task ProcessIslandsAsync(StarmapWorldObjectViewModel item)
        {
            await _semaphore.WaitAsync();
            try
            {
                var original = item.OriginalImage;
                var gradientImage = BitmapStarmapTransform.ApplyGradient(original, islandBrush);
                if (gradientImage.CanFreeze)
                    gradientImage.Freeze();

                var mapSize = Map.Size > Map.WorldBuffer ? Map.Size - Map.WorldBuffer : Map.Size;
                var outlineThickness = BorderThickness + 5 * Math.Max(gradientImage.PixelWidth, gradientImage.PixelHeight) / (mapSize);
                var result = await Task.Run(() =>
                {
                    var temp = BitmapStarmapTransform.GenerateOutline(gradientImage, outlineColor, outlineThickness);
                    if (temp.CanFreeze)
                        temp.Freeze();
                    return temp;
                });

                item.StarmapImage = result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task ProcessAsteroidsAsync(StarmapWorldObjectViewModel item)
        {
            await _semaphore.WaitAsync();
            try
            {
                var original = item.OriginalImage;
                var gradientImage = BitmapStarmapTransform.ApplyGradient(original, asteroidBrush);
                if (gradientImage.CanFreeze)
                    gradientImage.Freeze();

                var mapSize = Map.Size > Map.WorldBuffer ? Map.Size - Map.WorldBuffer : Map.Size;
                var outlineThickness = BorderThickness + 5 * Math.Max(gradientImage.PixelWidth, gradientImage.PixelHeight) / (mapSize);
                var result = await Task.Run(() =>
                {
                    var temp = BitmapStarmapTransform.GenerateOutline(gradientImage, outlineColor, outlineThickness);
                    if (temp.CanFreeze)
                        temp.Freeze();
                    return temp;
                });

                item.StarmapImage = result;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
