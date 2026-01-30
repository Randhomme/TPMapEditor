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
        private Brush islandBrush;
        private Brush asteroidBrush;
        private Color outlineColor;

        public WorldMap Map { get; }
        public double NegativeWorldBuffer { get => -Map.WorldBuffer; }
        public double BorderThickness { get; }
        public double NegativeBorderThickness { get; }
        public double EtheriumCurrentThickness { get; }
        public double NebulaBlurRadius { get; }
        public ICollectionView Asteroids { get; }
        public ICollectionView BlackHoles { get; }
        public ICollectionView Islands { get; }
        public ICollectionView Nebulas { get; }
        public ICollectionView EtheriumCurrents { get; }

        private readonly SemaphoreSlim _semaphore = new(4); // CPU limit

        public StarmapExportViewModel(WorldMap map)
        {
            islandBrush = new RadialGradientBrush(new()
            {
                new(Colors.Cyan, 0.25),
                new(Colors.DarkCyan, 0.75)
            })
            {
                RadiusX = 1,
                RadiusY = 1,
                Center = new(0.5,0.5)
            };
            asteroidBrush = new SolidColorBrush(Colors.Lime);
            outlineColor = Colors.White;
            this.Map = map;
            BorderThickness = 2 * (Map.Size - Map.WorldBuffer) / 512.0;
            NegativeBorderThickness = -BorderThickness;
            NebulaBlurRadius = 15 * (Map.Size - Map.WorldBuffer) / 512;
            EtheriumCurrentThickness = 11 * (Map.Size - Map.WorldBuffer) / 512;
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
