using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TPMapEditor.Controls
{
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DeleteSelectedButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MoveUpButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MoveDownButton", Type = typeof(Button))]
    public partial class CollectionEditorControl : Control
    {
        private IList? EditableList => ItemsSource as IList;
        private IList<ItemWrapper> SelectedWrappers => dataGrid?.SelectedItems.OfType<ItemWrapper>().ToList() ?? Array.Empty<ItemWrapper>().ToList();
        private INotifyCollectionChanged? observableItemsSource, _selectedItemsNotifier;
        private Type? itemsType;
        private DataGrid? dataGrid;
        private Button? addButton;
        private Button? deleteSelectedButton;
        private Button? moveUpButton;
        private Button? moveDownButton;
        private readonly RelayCommand addCommand;
        private readonly RelayCommand<object> deleteCommand;
        private readonly RelayCommand deleteSelectedCommand;
        private readonly RelayCommand moveUpCommand;
        private readonly RelayCommand moveDownCommand;
        private readonly DataGridColumn deleteButtonColumn;
        private bool isUpdatingSelection, wrappersReady, pendingSelectionApply;

        public CollectionEditorControl()
        {
            Columns = new ObservableCollection<DataGridColumn>();
            addCommand = new RelayCommand(AddNewItem, CanAddNewItem);
            deleteSelectedCommand = new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems);
            deleteCommand = new RelayCommand<object>(DeleteItem);
            moveUpCommand = new RelayCommand(() => MoveSelectedItems(-1), () => CanMoveSelectedItems(-1));
            moveDownCommand = new RelayCommand(() => MoveSelectedItems(1), () => CanMoveSelectedItems(1));
            var buttonFactory = new FrameworkElementFactory(typeof(Button));
            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetValue(Image.SourceProperty, new BitmapImage(new Uri("pack://application:,,,/TPMapEditor;component/Images/Cross.png")));
            buttonFactory.AppendChild(imageFactory);
            buttonFactory.SetValue(Button.CommandProperty, deleteCommand);
            buttonFactory.SetBinding(
                Button.CommandParameterProperty,
                new Binding("Item")
            );
            deleteButtonColumn = new DataGridTemplateColumn()
            {
                CellTemplate = new System.Windows.DataTemplate()
                {
                    VisualTree = buttonFactory,
                    
                },
            };
        }

        internal ObservableCollection<ItemWrapper>? Wrappers
        {
            get => (ObservableCollection<ItemWrapper>)GetValue(WrappersProperty);
        }

        private static readonly DependencyPropertyKey WrappersPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Wrappers), typeof(ObservableCollection<ItemWrapper>), typeof(CollectionEditorControl), new PropertyMetadata(null));

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

        public object? SelectedItem
        {
            get { return (object?)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CollectionEditorControl), new FrameworkPropertyMetadata(null, OnSelectedItemChanged) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        public Func<object>? Factory
        {
            get { return (Func<object>)GetValue(FactoryProperty); }
            set { SetValue(FactoryProperty, value); }
        }

        public static readonly DependencyProperty FactoryProperty =
            DependencyProperty.Register(nameof(Factory), typeof(Func<object>), typeof(CollectionEditorControl));

        public ObservableCollection<DataGridColumn> Columns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(ObservableCollection<DataGridColumn>), typeof(CollectionEditorControl));

        public bool OverrideColumnsFromTemplate
        {
            get { return (bool)GetValue(OverrideColumnsFromTemplateProperty); }
            set { SetValue(OverrideColumnsFromTemplateProperty, value); }
        }

        public static readonly DependencyProperty OverrideColumnsFromTemplateProperty =
            DependencyProperty.Register(nameof(OverrideColumnsFromTemplate), typeof(bool), typeof(CollectionEditorControl), new PropertyMetadata(true));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(CollectionEditorControl), new PropertyMetadata(null, OnSelectedItemsChanged));



        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CollectionEditorControl)d;
            control.DetachFromOldItemsSource(e.OldValue);
            control.AttachToNewItemsSource(e.NewValue);
            control.GetItemsType();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is ItemWrapper wrapper)
            {
                var control = (CollectionEditorControl)d;
                control.SelectedItem = wrapper.Item;
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CollectionEditorControl)d;

            control.OnSelectedItemsSourceChanged(e.OldValue, e.NewValue);

            control.ApplySelectedItemsToDataGridSelectionIfPossible();
        }

        private void OnSelectedItemsSourceChanged(object? oldValue, object? newValue)
        {
            if (_selectedItemsNotifier != null)
                _selectedItemsNotifier.CollectionChanged -= OnSelectedItemsCollectionChanged;

            _selectedItemsNotifier = newValue as INotifyCollectionChanged;

            if (_selectedItemsNotifier != null)
                _selectedItemsNotifier.CollectionChanged += OnSelectedItemsCollectionChanged;
        }

        private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ApplySelectedItemsToDataGridSelectionIfPossible();
        }

        private void DetachFromOldItemsSource(object oldValue)
        {
            wrappersReady = false;

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

            wrappersReady = true;
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var index = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            Wrappers?.Insert(index, new ItemWrapper(item));
                            index++;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        RemoveWrapper(item);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetWrappers();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (var item in e.OldItems)
                            RemoveWrapper(item);
                        var index = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            Wrappers?.Insert(index, new ItemWrapper(item));
                            index++;
                        }
                    }
                    break;
            }
            moveUpCommand.NotifyCanExecuteChanged();
            moveDownCommand.NotifyCanExecuteChanged();
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
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                dataGrid.LoadingRow -= DataGrid_LoadingRow;
                dataGrid.Columns.Clear();
                dataGrid.SelectedItems.Clear();
            }

            base.OnApplyTemplate();

            dataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;
            addButton = GetTemplateChild("PART_AddButton") as Button;
            deleteSelectedButton = GetTemplateChild("PART_DeleteSelectedButton") as Button;
            moveUpButton = GetTemplateChild("PART_MoveUpButton") as Button;
            moveDownButton = GetTemplateChild("PART_MoveDownButton") as Button;
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                dataGrid.LoadingRow += DataGrid_LoadingRow;
                if (pendingSelectionApply)
                    ApplySelectedItemsToDataGridSelectionIfPossible();
                if (Columns.Count > 0)
                {
                    if (OverrideColumnsFromTemplate)
                        dataGrid.Columns.Clear();
                    foreach (var column in Columns)
                    {
                        //dataGrid.Columns.Add(CloneColumn(column));
                        dataGrid.Columns.Add(column);
                    }
                }
                dataGrid.Columns.Add(deleteButtonColumn);
            }
            if (addButton != null)
                addButton.Command = addCommand;
            if (deleteSelectedButton != null)
                deleteSelectedButton.Command = deleteSelectedCommand;
            if (moveUpButton != null)
                moveUpButton.Command = moveUpCommand;
            if (moveDownButton != null)
                moveDownButton.Command = moveDownCommand;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyDataGridSelectionToSelectedItemsIfPossible(e.AddedItems, e.RemovedItems);
            moveUpCommand.NotifyCanExecuteChanged();
            moveDownCommand.NotifyCanExecuteChanged();
            deleteSelectedCommand.NotifyCanExecuteChanged();
        }

        private void ApplyDataGridSelectionToSelectedItemsIfPossible(IList addedItems, IList removedItems)
        {
            if (isUpdatingSelection || dataGrid == null || SelectedItems == null)
                return;

            isUpdatingSelection = true;

            try
            {
                foreach (var w in addedItems.OfType<ItemWrapper>())
                    SelectedItems.Add(w.Item);
                foreach (var w in removedItems.OfType<ItemWrapper>())
                    SelectedItems.Remove(w.Item);
            }
            finally
            {
                isUpdatingSelection = false;
            }
        }

        private void ApplySelectedItemsToDataGridSelectionIfPossible()
        {
            if (!wrappersReady || dataGrid == null || Wrappers == null)
            {
                pendingSelectionApply = true;
                return;
            }

            if (isUpdatingSelection)
                return;

            isUpdatingSelection = true;

            try
            {
                dataGrid.SelectedItems.Clear();

                foreach(var item in SelectedItems)
                {
                    var wrapper = Wrappers.FirstOrDefault((w) => w.Item == item);
                    dataGrid.SelectedItems.Add(wrapper);
                }
                if (dataGrid.SelectedItems.Count > 0)
                    dataGrid.ScrollIntoView(dataGrid.SelectedItems[dataGrid.SelectedItems.Count - 1]);
            }
            finally
            {
                isUpdatingSelection = false;
                pendingSelectionApply = false;
            }
        }

        private void GetItemsType()
        {
            itemsType = ItemsSource?.GetType().GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))?.GetGenericArguments()[0];
        }

        private object? CreateItem()
        {
            if (Factory != null)
                return Factory.Invoke();

            if (itemsType == null)
                return null;

            var ctor = itemsType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new InvalidOperationException(
                    $"No factory provided and type {itemsType.Name} has no parameterless constructor.");

            return ctor.Invoke(null);
        }

        private void AddNewItem()
        {
            EditableList?.Add(CreateItem());
        }

        private bool CanAddNewItem()
        {
            var result =  Factory != null || itemsType?.GetConstructor(Type.EmptyTypes) != null;
            if (!result && addButton != null)
                addButton.Visibility = Visibility.Collapsed;
            return result;
        }

        private void DeleteItem(object? item)
        {
            EditableList?.Remove(item);
        }

        private void DeleteSelectedItems()
        {
            foreach(var wrapper in SelectedWrappers)
            {
                EditableList?.Remove(wrapper.Item);
            }
        }

        private bool CanDeleteSelectedItems()
        {
            return dataGrid?.SelectedItems.Count > 0;
        }

        private void MoveSelectedItems(int direction)
        {
            var selectedItems = SelectedWrappers;

            if (selectedItems.Count <= 0 || EditableList == null)
                return;

            var items = selectedItems.Select(w => w.Item).ToList();
            var indices = items
                .Select(i => EditableList.IndexOf(i))
                .OrderBy(i => i)
                .ToList();

            if (direction < 0)
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    var index = indices[i];
                    var item = EditableList[index];
                    EditableList.RemoveAt(index);
                    EditableList.Insert(index + direction, item);
                }
            }
            else
            {
                for (int i = indices.Count - 1; i >= 0; i--)
                {
                    var index = indices[i];
                    var item = EditableList[index];
                    EditableList.RemoveAt(index);
                    EditableList.Insert(index + direction, item);
                }
            }

            RestoreSelection(items);

            moveUpCommand.NotifyCanExecuteChanged();
            moveDownCommand.NotifyCanExecuteChanged();
        }

        private bool CanMoveSelectedItems(int direction)
        {
            var selectedItems = SelectedWrappers;
            
            if (selectedItems.Count <= 0 || EditableList == null)
                return false;

            var indices = selectedItems.Select(w => EditableList.IndexOf(w.Item)).OrderBy(i => i).ToList();

            if (indices.Any(i => i < 0))
                return false;

            return direction < 0 ? indices.First() > 0 : indices.Last() < EditableList.Count - 1;
        }

        private void RestoreSelection(IEnumerable<object> items)
        {
            if (dataGrid != null)
            {
                dataGrid.SelectedItems.Clear();

                foreach (var wrapper in Wrappers.Where(w => items.Contains(w.Item)))
                    dataGrid.SelectedItems.Add(wrapper);
            }
        }

        //private DataGridColumn? CloneColumn(DataGridColumn column)
        //{
        //    return column switch
        //    {
        //        DataGridTextColumn text => CloneTextColumn(text),
        //        DataGridCheckBoxColumn check => CloneCheckBoxColumn(check),
        //        DataGridTemplateColumn template => CloneTemplateColumn(template),
        //        _ => null,
        //    };
        //}

        //private DataGridColumn CloneTextColumn(DataGridTextColumn column)
        //{
        //    return new DataGridTextColumn()
        //    {
        //        Binding = column.Binding
        //    };
        //}
        //private DataGridColumn CloneCheckBoxColumn(DataGridCheckBoxColumn column)
        //{
        //    return new DataGridTextColumn();
        //}
        //private DataGridColumn CloneTemplateColumn(DataGridTemplateColumn column)
        //{
        //    return new DataGridTextColumn();
        //}
    }

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


    //public static class DataGridColumnRegistry
    //{
    //    private static readonly Dictionary<Type, Func<IList<DataGridColumn>>> _map
    //        = new();

    //    public static void Register<T>(Func<IList<DataGridColumn>> factory)
    //    {
    //        _map[typeof(T)] = factory;
    //    }

    //    public static bool TryGet(Type type, out IList<DataGridColumn> columns)
    //    {
    //        if (_map.TryGetValue(type, out var factory))
    //        {
    //            columns = factory();
    //            return true;
    //        }

    //        columns = null!;
    //        return false;
    //    }
    //}
}
