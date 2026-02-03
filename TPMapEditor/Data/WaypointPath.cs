using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.Data
{
    public partial class WaypointPath : MultiPointNamedMapObject<WaypointPathPoint>
    {
        public static string[] DefaultName => new string[] { "NO PATH", "PATH NAME" };

        public static WaypointPath DefaultWaypointPath { get; } = new(null, DefaultName[0], null);

        private readonly ObservableCollection<WaypointPathPoint> selectedItems = new();
        private readonly ICopyPasteService waypointPathPointCopyPasteService;
        public ICollection<WaypointPathPoint> SelectedItems { get => selectedItems; }

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public Func<WaypointPathPoint> WaypointPathPointFactory { get; }

        public WaypointPath(WorldMap map, string name, ICopyPasteService waypointPathPointCopyPasteService) : base(map, name)
        {
            WaypointPathPointFactory = () => new(this, 0, 0, 0);
            Color = Colors.Black;
            this.waypointPathPointCopyPasteService = waypointPathPointCopyPasteService;
            selectedItems.CollectionChanged += (s, e) =>
            {
                CopyCommand.NotifyCanExecuteChanged();
            };
            if (waypointPathPointCopyPasteService != null)
                PropertyChangedEventManager.AddHandler(waypointPathPointCopyPasteService, (s, e) => { PasteCommand.NotifyCanExecuteChanged(); }, string.Empty);
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WaypointPaths)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return DefaultName.Contains(name);
        }

        public override ICopiableMapObject Copy()
        {
            var copy = new WaypointPath(Map, GenerateName($"{Name}_", Map.WaypointPaths), waypointPathPointCopyPasteService)
            {
                Color = this.Color
            };
            for (int i = 0; i < Points.Count; i++)
            {
                var p = (WaypointPathPoint)Points[i].Copy();
                p.IsSelected = p.IsLastSelected = false;
                p.Parent = copy;
                copy.Points.Add(p);
            }
            return copy;
        }

        [RelayCommand(CanExecute = nameof(CanCopy))]
        private void OnCopy()
        {
            waypointPathPointCopyPasteService.Copy(SelectedItems);
        }

        [RelayCommand(CanExecute = nameof(CanPaste))]
        private void OnPaste()
        {
            var pastedItems = waypointPathPointCopyPasteService.Paste<WaypointPathPoint>();
            foreach (var item in pastedItems)
            {
                item.Parent = this;
                item.IsSelected = item.IsLastSelected = false;
                Points.Add(item);
            }
        }

        private bool CanCopy() => SelectedItems.Count > 0;

        private bool CanPaste() => waypointPathPointCopyPasteService.ClipboardCount > 0;
    }
}
