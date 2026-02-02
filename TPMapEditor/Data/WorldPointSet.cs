using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TPMapEditor.Interfaces;
using TPMapEditor.Interfaces.Implementations;
using TPMapEditor.Services;

namespace TPMapEditor.Data
{
    public partial class WorldPointSet : MultiPointNamedMapObject<WorldPoint>
    {
        public static string DefaultName => "POINT SET";

        public static WorldPointSet DefaultWorldPointSet { get; } = new(null, DefaultName, null);

        private bool canPaste = false;
        private readonly ObservableCollection<WorldPoint> selectedItems = new();
        private readonly ICopyPasteService copyPasteService;
        public ICollection<WorldPoint> SelectedItems { get => selectedItems; }

        [ObservableProperty]
        private Color color = Colors.Black;

        public Func<WorldPoint> WorldPointFactory { get; }

        public WorldPointSet(WorldMap map, string name, ICopyPasteService copyPasteService) : base(map, name)
        {
            WorldPointFactory = () => new(this, 0, 0, 0, 0);
            this.copyPasteService = copyPasteService;
            selectedItems.CollectionChanged += (s, e) =>
            {
                CopyCommand.NotifyCanExecuteChanged();
            };
            copyPasteService?.ClearClipboard();
            canPaste = false;
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.WorldPointSets)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }

        public override bool IsDefaultName(string name)
        {
            return name.Equals(DefaultName);
        }

        public override ICopiableMapObject Copy()
        {
            var copy = new WorldPointSet(Map, GenerateName($"{Name}_", Map.WorldPointSets), copyPasteService)
            {
                Color = this.Color
            };
            for (int i = 0; i < Points.Count; i++)
            {
                var p = (WorldPoint)Points[i].Copy();
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
            var pastedItems = copyPasteService.Paste<WorldPoint>();
            foreach (var item in pastedItems)
            {
                Points.Add(item);
            }
        }

        private bool CanCopy() => SelectedItems.Count > 0;

        private bool CanPaste() => canPaste;
    }
}
