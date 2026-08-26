using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        private readonly IList<StarmapWorldObjectViewModel> worldObjectViewModels;

        public WorldMap Map { get; }
        public double NegativeWorldBuffer { get => -Map.WorldBuffer; }
        public double BorderThickness { get; } = 3;
        public double TranslateTransformIsland { get; } = -0.5;
        public double ScaleDownTransformIsland { get; } = 0.001;
        public double NegativeBorderThickness { get; }
        public double EtheriumCurrentThickness { get; } = 14;
        public double EtheriumCurrentShadowThickness { get; } = 6;
        public double NebulaBlurRadius { get; } = 20;
        public double IslandShadowBlurRadius { get; } = 25;
        public double IslandInnerShadowBlurRadius { get; } = 0;
        public ICollectionView Asteroids { get; }
        public ICollectionView BlackHoles { get; }
        public ICollectionView Islands { get; }
        public ICollectionView Nebulas { get; }
        public ICollectionView EtheriumCurrents { get; }

        private readonly SemaphoreSlim _semaphore = new(4); // CPU limit
        private readonly CancellationTokenSource _cts = new();

        public StarmapExportViewModel(WorldMap map)
        {
            islandBrush = new SolidColorBrush(Colors.DarkCyan);
            asteroidBrush = new SolidColorBrush(Colors.LimeGreen);
            outlineColor = Colors.White;
            this.Map = map;
            BorderThickness *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            NegativeBorderThickness = -BorderThickness;
            NebulaBlurRadius *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            EtheriumCurrentThickness *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            EtheriumCurrentShadowThickness *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            IslandShadowBlurRadius *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            IslandInnerShadowBlurRadius *= (Map.Size - Map.WorldBuffer * 2) / 512.0;
            TranslateTransformIsland *= (Map.Size - Map.WorldBuffer * 2) / 512;
            ScaleDownTransformIsland = 1 - ScaleDownTransformIsland * (Map.Size - Map.WorldBuffer * 2) / 512.0;
            worldObjectViewModels = new List<StarmapWorldObjectViewModel>(map.WorldObjects.Select((w) => new StarmapWorldObjectViewModel(w)));
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
            var token = _cts.Token;
            var islands = Islands.Cast<StarmapWorldObjectViewModel>().ToList();
            var asteroids = Asteroids.Cast<StarmapWorldObjectViewModel>().ToList();
            var islandsTasks = islands.Select(i => ProcessIslandsAsync(i, token));
            var asteroidsTasks = asteroids.Select(a => ProcessAsteroidsAsync(a, token));
            await Task.WhenAll(islandsTasks);
            await Task.WhenAll(asteroidsTasks);
        }

        private async Task ProcessIslandsAsync(StarmapWorldObjectViewModel item, CancellationToken token)
        {
            var entered = false;
            try
            {
                await _semaphore.WaitAsync(token);
                entered = true;

                token.ThrowIfCancellationRequested();
                var original = item.OriginalImage;
                var gradientImage = BitmapStarmapTransform.ApplyGradient(original, islandBrush);
                if (gradientImage.CanFreeze)
                    gradientImage.Freeze();

                var mapSize = Map.Size > Map.WorldBuffer ? Map.Size - Map.WorldBuffer : Map.Size;
                var outlineThickness = Math.Min(BorderThickness, Math.Min(gradientImage.PixelWidth, gradientImage.PixelHeight));
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    var temp = BitmapStarmapTransform.GenerateOutline(gradientImage, outlineColor, outlineThickness, token);
                    if (temp.CanFreeze)
                        temp.Freeze();
                    return temp;
                }, token);

                item.StarmapImage = result;
            }
            catch (OperationCanceledException)
            {
                // Cancellation requested - stop processing
            }
            finally
            {
                if (entered)
                    _semaphore.Release();
            }
        }

        private async Task ProcessAsteroidsAsync(StarmapWorldObjectViewModel item, CancellationToken token)
        {
            var entered = false;
            try
            {
                await _semaphore.WaitAsync(token);
                entered = true;

                token.ThrowIfCancellationRequested();
                var original = item.OriginalImage;
                var gradientImage = BitmapStarmapTransform.ApplyGradient(original, asteroidBrush);
                if (gradientImage.CanFreeze)
                    gradientImage.Freeze();

                var mapSize = Map.Size > Map.WorldBuffer ? Map.Size - Map.WorldBuffer : Map.Size;
                var outlineThickness = Math.Min(BorderThickness, Math.Min(gradientImage.PixelWidth, gradientImage.PixelHeight));
                var result = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    var temp = BitmapStarmapTransform.GenerateOutline(gradientImage, outlineColor, outlineThickness, token);
                    if (temp.CanFreeze)
                        temp.Freeze();
                    return temp;
                }, token);

                item.StarmapImage = result;
            }
            catch (OperationCanceledException)
            {
                // Cancellation requested - stop processing
            }
            finally
            {
                if (entered)
                    _semaphore.Release();
            }
        }

        public void Close()
        {
            _cts.Cancel();
            _cts.Dispose();
            //_semaphore.Release();
            //_semaphore.Dispose();
            worldObjectViewModels.Clear();
        }
    }
}
