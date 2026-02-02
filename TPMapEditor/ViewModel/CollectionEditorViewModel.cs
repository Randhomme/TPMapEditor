using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TPMapEditor.Interfaces;
using TPMapEditor.Services;

namespace TPMapEditor.ViewModel
{
    public partial class CollectionEditorViewModel<T> : ObservableObject where T : ISelectableMapObject
    {
        private bool canPaste = false;
        private readonly ICopyPasteService copyPasteService;
        private readonly ObservableCollection<T> selectedItems = new();
        public ICollection<T> ItemsSource { get; }
        public ICollection<T> SelectedItems { get => selectedItems; }
        public Func<object> Factory { get; }
        public bool GridOnlyMode { get; }

        public CollectionEditorViewModel(ICollection<T> itemSource, Func<object> factory, ICopyPasteService copyPasteService, bool gridOnlyMode = false)
        {
            this.ItemsSource = itemSource;
            this.Factory = factory;
            this.copyPasteService = copyPasteService;
            this.GridOnlyMode = gridOnlyMode;
            selectedItems.CollectionChanged += (s, e) =>
            {
                CopyCommand.NotifyCanExecuteChanged();
            };
            copyPasteService.ClearClipboard();
            canPaste = false;
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
            var pastedItems = copyPasteService.Paste<T>();
            foreach (var item in pastedItems)
            {
                ItemsSource.Add(item);
            }
        }

        private bool CanCopy() => SelectedItems.Count > 0;

        private bool CanPaste() => canPaste;
    }
}
