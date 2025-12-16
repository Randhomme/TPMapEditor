using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    public partial class CollectionEditor<T> : ObservableObject where T : INotifyPropertyChanged
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveUpObjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveDownObjectCommand))]
        private CollectionEditorObjectWrapper<T>? selectedItem;

        private readonly Func<T> factory;

        public ObservableCollection<T> Source { get; }
        public ObservableCollection<CollectionEditorObjectWrapper<T>> Items { get; }

        public CollectionEditor(ObservableCollection<T> source, Func<T> factory)
        {
            Source = source;
            this.factory = factory;
            Items = new ObservableCollection<CollectionEditorObjectWrapper<T>>(Source.Select(item => new CollectionEditorObjectWrapper<T>(item)));
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CollectionEditorObjectWrapper<T> item in e.NewItems)
                    Source.Add(item.Item);

            if (e.OldItems != null)
                foreach (CollectionEditorObjectWrapper<T> item in e.OldItems)
                {
                    var wrapper = Source.First(w => Equals(w, item.Item));
                    Source.Remove(wrapper);
                }
        }

        [RelayCommand]
        private void OnAddObject()
        {
            var wrapper = new CollectionEditorObjectWrapper<T>(factory());
            Items.Add(wrapper);
            SelectedItem = wrapper;
        }

        [RelayCommand]
        private void OnRemoveObject()
        {
            if (SelectedItem != null)
                Items.Remove(SelectedItem);
        }

        [RelayCommand(CanExecute = nameof(CanMoveUpWorldRule))]
        private void OnMoveUpObject()
        {
            var index = Items.IndexOf(SelectedItem!);
            Items.Move(index, index - 1);
            MoveUpObjectCommand.NotifyCanExecuteChanged();
            MoveDownObjectCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveDownWorldRule))]
        private void OnMoveDownObject()
        {
            var index = Items.IndexOf(SelectedItem!);
            Items.Move(index, index + 1);
            MoveUpObjectCommand.NotifyCanExecuteChanged();
            MoveDownObjectCommand.NotifyCanExecuteChanged();
        }

        private bool CanMoveUpWorldRule()
        {
            return SelectedItem != null && Items.IndexOf(SelectedItem) > 0;
        }

        private bool CanMoveDownWorldRule()
        {
            return SelectedItem != null && Items.IndexOf(SelectedItem) < Items.Count - 1;
        }
    }

    public class CollectionEditorObjectWrapper : ObservableObject { }

    public class CollectionEditorObjectWrapper<T> : CollectionEditorObjectWrapper where T : INotifyPropertyChanged
    {
        public T Item { get; }

        public string Display => Item.ToString();

        public CollectionEditorObjectWrapper(T item)
        {
            Item = item;
            Item.PropertyChanged += Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Display));
        }

        //public override string ToString()
        //{
        //    return Display;
        //}
    }
}
