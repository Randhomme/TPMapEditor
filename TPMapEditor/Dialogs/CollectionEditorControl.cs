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

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for CollectionEditorControl.xaml
    /// </summary>
    public partial class CollectionEditorControl : Control
    {
        public bool GridOnlyMode
        {
            get => (bool)GetValue(GridOnlyModeProperty);
            set => SetValue(GridOnlyModeProperty, value);
        }

        public static readonly DependencyProperty GridOnlyModeProperty =
            DependencyProperty.Register(nameof(GridOnlyMode), typeof(bool), typeof(CollectionEditorControl), new PropertyMetadata(false));

        //public IEnumerable ItemsSource
        //{
        //    get => (IEnumerable)GetValue(ItemsSourceProperty);
        //    set => SetValue(ItemsSourceProperty, value);
        //}

        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CollectionEditorControl), new PropertyMetadata(null, OnItemsSourceChanged));

        //private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}

        //public object SelectedItem
        //{
        //    get => GetValue(SelectedItemProperty);
        //    set => SetValue(SelectedItemProperty, value);
        //}

        //public static readonly DependencyProperty SelectedItemProperty =
        //    DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CollectionEditorControl));

        //public ICommand AddCommand
        //{
        //    get { return (ICommand)GetValue(AddCommandProperty); }
        //    set { SetValue(AddCommandProperty, value); }
        //}

        //public static readonly DependencyProperty AddCommandProperty =
        //    DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(CollectionEditorControl));

        //public ICommand RemoveCommand
        //{
        //    get { return (ICommand)GetValue(RemoveCommandProperty); }
        //    set { SetValue(RemoveCommandProperty, value); }
        //}

        //public static readonly DependencyProperty RemoveCommandProperty =
        //    DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand), typeof(CollectionEditorControl));

        //public ICommand MoveUpCommand
        //{
        //    get { return (ICommand)GetValue(MoveUpCommandProperty); }
        //    set { SetValue(MoveUpCommandProperty, value); }
        //}

        //public static readonly DependencyProperty MoveUpCommandProperty =
        //    DependencyProperty.Register(nameof(MoveUpCommand), typeof(ICommand), typeof(CollectionEditorControl));


        //public ICommand MoveDownCommand
        //{
        //    get { return (ICommand)GetValue(MoveDownCommandProperty); }
        //    set { SetValue(MoveDownCommandProperty, value); }
        //}

        //public static readonly DependencyProperty MoveDownCommandProperty =
        //    DependencyProperty.Register(nameof(MoveDownCommand), typeof(ICommand), typeof(CollectionEditorControl));



        //public Func<INotifyPropertyChanged> Factory
        //{
        //    get { return (Func<INotifyPropertyChanged>)GetValue(FactoryProperty); }
        //    set { SetValue(FactoryProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Factory.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty FactoryProperty =
        //    DependencyProperty.Register(nameof(Factory), typeof(Func<INotifyPropertyChanged>), typeof(CollectionEditorControl));



    }

    public partial class CollectionEditorControlContext<T> : ObservableObject where T : INotifyPropertyChanged
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveUpCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveDownCommand))]
        private CollectionEditorObjectWrapper<T>? selectedItem;

        [ObservableProperty]
        private bool gridOnlyMode;

        private readonly Func<T> factory;

        public ObservableCollection<T> Source { get; }
        public ObservableCollection<CollectionEditorObjectWrapper<T>> Items { get; }

        public CollectionEditorControlContext(ObservableCollection<T> source, Func<T> factory, bool gridOnlyMode = false)
        {
            Source = source;
            this.factory = factory;
            this.gridOnlyMode = gridOnlyMode;
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
        private void OnAdd()
        {
            var wrapper = new CollectionEditorObjectWrapper<T>(factory());
            Items.Add(wrapper);
            SelectedItem = wrapper;
        }

        [RelayCommand]
        private void OnRemove()
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
                SelectedItem = null;
            }
        }

        [RelayCommand(CanExecute = nameof(CanMoveUp))]
        private void OnMoveUp()
        {
            var index = Items.IndexOf(SelectedItem!);
            Items.Move(index, index - 1);
            MoveUpCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveDown))]
        private void OnMoveDown()
        {
            var index = Items.IndexOf(SelectedItem!);
            Items.Move(index, index + 1);
            MoveUpCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
        }

        private bool CanMoveUp()
        {
            return SelectedItem != null && Items.IndexOf(SelectedItem) > 0;
        }

        private bool CanMoveDown()
        {
            return SelectedItem != null && Items.IndexOf(SelectedItem) < Items.Count - 1;
        }
    }

    public class CollectionEditorObjectWrapper<T> : ObservableObject where T : INotifyPropertyChanged
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
