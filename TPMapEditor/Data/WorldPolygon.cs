using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;
using TPMapEditor.Services.Implementations;

namespace TPMapEditor.Data
{
    public partial class WorldPolygon : MultiPointNamedMapObject<WorldPolygonPoint>
    {
        private bool canPaste = false;
        private readonly ObservableCollection<WorldPolygonPoint> selectedItems = new();
        private readonly ICopyPasteService copyPasteService;
        public ICollection<WorldPolygonPoint> SelectedItems { get => selectedItems; }

        [ObservableProperty]
        private Color color; // for visual purpose only, not used in the map file
        public Func<WorldPolygonPoint> WorldPolygonPointFactory { get; }
        public WorldPolygon(WorldMap map, string name, ICopyPasteService copyPasteService) : base(map, name)
        {
            WorldPolygonPointFactory = () => new(this, 0, 0);
            this.color = Colors.Black;
            this.copyPasteService = copyPasteService;
            selectedItems.CollectionChanged += (s, e) =>
            {
                CopyCommand.NotifyCanExecuteChanged();
            };
            copyPasteService.ClearClipboard();
            canPaste = false;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WorldPolygons)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override ICopiableMapObject Copy()
        {
            var copy = new WorldPolygon(Map, GenerateName($"{Name}_", Map.WaypointPaths), copyPasteService)
            {
                Color = this.Color
            };
            for (int i = 0; i < Points.Count; i++)
            {
                var p = (WorldPolygonPoint)Points[i].Copy();
                p.IsSelected = p.IsLastSelected = false;
                p.Parent = copy;
                copy.Points.Add(p);
            }
            return copy;
        }

        [RelayCommand(CanExecute = nameof(CanCopy))]
        private void OnCopy()
        {
            copyPasteService.Copy(SelectedItems);
            canPaste = true;
            PasteCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPaste))]
        private void OnPaste()
        {
            var pastedItems = copyPasteService.Paste<WorldPolygonPoint>();
            foreach (var item in pastedItems)
            {
                Points.Add(item);
            }
        }

        private bool CanCopy() => SelectedItems.Count > 0;

        private bool CanPaste() => canPaste;
    }
}
