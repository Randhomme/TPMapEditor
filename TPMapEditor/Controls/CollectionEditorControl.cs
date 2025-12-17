using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TPMapEditor.Controls
{
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MoveUpButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MoveDownButton", Type = typeof(Button))]
    public partial class CollectionEditorControl : Control
    {
        private IList? editableList => ItemsSource as IList;
        private INotifyCollectionChanged? observableItemsSource;

        private Button? addButton;
        private Button? moveUpButton;
        private Button? moveDownButton;
        private ICommand addCommand;

        public CollectionEditorControl()
        {
            addCommand = new RelayCommand(AddNewItem);
        }

        internal ObservableCollection<ItemWrapper>? Wrappers
        {
            get => (ObservableCollection<ItemWrapper>)GetValue(WrappersProperty);
        }

        private static readonly DependencyPropertyKey WrappersPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Wrappers),
                typeof(ObservableCollection<ItemWrapper>),
                typeof(CollectionEditorControl),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty WrappersProperty =
            WrappersPropertyKey.DependencyProperty;

        public bool GridOnlyMode
        {
            get => (bool)GetValue(GridOnlyModeProperty);
            set => SetValue(GridOnlyModeProperty, value);
        }

        public static readonly DependencyProperty GridOnlyModeProperty =
            DependencyProperty.Register(nameof(GridOnlyMode), typeof(bool), typeof(CollectionEditorControl), new PropertyMetadata(false));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CollectionEditorControl), new PropertyMetadata(null, OnItemsSourceChanged));

        internal ItemWrapper? SelectedWrapper
        {
            get => (ItemWrapper)GetValue(SelectedWrapperProperty);
            set => SetValue(SelectedWrapperProperty, value);
        }

        internal static readonly DependencyProperty SelectedWrapperProperty =
            DependencyProperty.Register(
                nameof(SelectedWrapper),
                typeof(ItemWrapper),
                typeof(CollectionEditorControl),
                new PropertyMetadata(OnSelectedWrapperChanged));

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            private set => SetValue(SelectedItemPropertyKey, value);
        }

        private static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectedItem),
                typeof(object),
                typeof(CollectionEditorControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
            SelectedItemPropertyKey.DependencyProperty;

        private static void OnSelectedWrapperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CollectionEditorControl)d;
            control.SelectedItem = (e.NewValue as ItemWrapper)?.Item;
        }

        public Func<object> Factory
        {
            get { return (Func<object>)GetValue(FactoryProperty); }
            set { SetValue(FactoryProperty, value); }
        }

        public static readonly DependencyProperty FactoryProperty =
            DependencyProperty.Register(nameof(Factory), typeof(Func<object>), typeof(CollectionEditorControl));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CollectionEditorControl)d;
            control.DetachFromOldItemsSource(e.OldValue);
            control.AttachToNewItemsSource(e.NewValue);
        }

        private void DetachFromOldItemsSource(object oldValue)
        {
            if (observableItemsSource != null)
                observableItemsSource.CollectionChanged -= OnItemsSourceCollectionChanged;

            if (Wrappers != null)
            {
                foreach (var wrapper in Wrappers)
                    wrapper.Dispose();

                Wrappers.Clear();
            }

            observableItemsSource = null;
        }

        private void AttachToNewItemsSource(object newValue)
        {
            if (Wrappers == null)
                SetValue(WrappersPropertyKey, new ObservableCollection<ItemWrapper>());

            if (newValue is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                    Wrappers!.Add(new ItemWrapper(item));
            }

            observableItemsSource = newValue as INotifyCollectionChanged;
            if (observableItemsSource != null)
                observableItemsSource.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        Wrappers?.Add(new ItemWrapper(item));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        RemoveWrapper(item);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetWrappers();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                        RemoveWrapper(item);
                    foreach (var item in e.NewItems)
                        Wrappers?.Add(new ItemWrapper(item));
                    break;
            }
        }

        private void RemoveWrapper(object item)
        {
            var wrapper = Wrappers.FirstOrDefault(w => ReferenceEquals(w.Item, item));
            if (wrapper != null)
            {
                wrapper.Dispose();
                Wrappers?.Remove(wrapper);
            }
        }

        private void ResetWrappers()
        {
            if (Wrappers != null)
            {
                foreach (var wrapper in Wrappers)
                    wrapper.Dispose();

                Wrappers.Clear();

                if (ItemsSource is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                        Wrappers.Add(new ItemWrapper(item));
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            addButton = GetTemplateChild("PART_AddButton") as Button;
            if (addButton != null)
                addButton.Command = addCommand;
        }

        private void AddNewItem()
        {
            editableList?.Add(Factory());
        }

    }

    //public partial class CollectionEditorControlContext<T> : ObservableObject where T : INotifyPropertyChanged
    //{
    //    [ObservableProperty]
    //    [NotifyCanExecuteChangedFor(nameof(MoveUpCommand))]
    //    [NotifyCanExecuteChangedFor(nameof(MoveDownCommand))]
    //    private CollectionEditorObjectWrapper<T>? selectedItem;

    //    [ObservableProperty]
    //    private bool gridOnlyMode;

    //    private readonly Func<T> factory;

    //    public ObservableCollection<T> Source { get; }
    //    public ObservableCollection<CollectionEditorObjectWrapper<T>> Items { get; }

    //    public CollectionEditorControlContext(ObservableCollection<T> source, Func<T> factory, bool gridOnlyMode = false)
    //    {
    //        Source = source;
    //        this.factory = factory;
    //        this.gridOnlyMode = gridOnlyMode;
    //        Items = new ObservableCollection<CollectionEditorObjectWrapper<T>>(Source.Select(item => new CollectionEditorObjectWrapper<T>(item)));
    //        Items.CollectionChanged += Items_CollectionChanged;
    //    }

    //    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        if (e.NewItems != null)
    //            foreach (CollectionEditorObjectWrapper<T> item in e.NewItems)
    //                Source.Add(item.Item);

    //        if (e.OldItems != null)
    //            foreach (CollectionEditorObjectWrapper<T> item in e.OldItems)
    //            {
    //                var wrapper = Source.First(w => Equals(w, item.Item));
    //                Source.Remove(wrapper);
    //            }
    //    }

    //    [RelayCommand]
    //    private void OnAdd()
    //    {
    //        var wrapper = new CollectionEditorObjectWrapper<T>(factory());
    //        Items.Add(wrapper);
    //        SelectedItem = wrapper;
    //    }

    //    [RelayCommand]
    //    private void OnRemove()
    //    {
    //        if (SelectedItem != null)
    //        {
    //            Items.Remove(SelectedItem);
    //            SelectedItem = null;
    //        }
    //    }

    //    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    //    private void OnMoveUp()
    //    {
    //        var index = Items.IndexOf(SelectedItem!);
    //        Items.Move(index, index - 1);
    //        MoveUpCommand.NotifyCanExecuteChanged();
    //        MoveDownCommand.NotifyCanExecuteChanged();
    //    }

    //    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    //    private void OnMoveDown()
    //    {
    //        var index = Items.IndexOf(SelectedItem!);
    //        Items.Move(index, index + 1);
    //        MoveUpCommand.NotifyCanExecuteChanged();
    //        MoveDownCommand.NotifyCanExecuteChanged();
    //    }

    //    private bool CanMoveUp()
    //    {
    //        return SelectedItem != null && Items.IndexOf(SelectedItem) > 0;
    //    }

    //    private bool CanMoveDown()
    //    {
    //        return SelectedItem != null && Items.IndexOf(SelectedItem) < Items.Count - 1;
    //    }
    //}

    public sealed class ItemWrapper : ObservableObject, IDisposable
    {
        public object Item { get; }

        public string Display => Item.ToString();

        public ItemWrapper(object item)
        {
            Item = item;

            if (item is INotifyPropertyChanged npc)
                npc.PropertyChanged += OnItemPropertyChanged;
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Display));
        }

        public void Dispose()
        {
            if (Item is INotifyPropertyChanged npc)
                npc.PropertyChanged -= OnItemPropertyChanged;
        }
    }


    public static class DataGridColumnRegistry
    {
        private static readonly Dictionary<Type, Func<IList<DataGridColumn>>> _map
            = new();

        public static void Register<T>(Func<IList<DataGridColumn>> factory)
        {
            _map[typeof(T)] = factory;
        }

        public static bool TryGet(Type type, out IList<DataGridColumn> columns)
        {
            if (_map.TryGetValue(type, out var factory))
            {
                columns = factory();
                return true;
            }

            columns = null!;
            return false;
        }
    }
}
