using System;
using System.Collections;
using System.Collections.Generic;
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

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(CollectionEditorControl));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CollectionEditorControl));

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(CollectionEditorControl));

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand), typeof(CollectionEditorControl));

        public ICommand MoveUpCommand
        {
            get { return (ICommand)GetValue(MoveUpCommandProperty); }
            set { SetValue(MoveUpCommandProperty, value); }
        }

        public static readonly DependencyProperty MoveUpCommandProperty =
            DependencyProperty.Register(nameof(MoveUpCommand), typeof(ICommand), typeof(CollectionEditorControl));


        public ICommand MoveDownCommand
        {
            get { return (ICommand)GetValue(MoveDownCommandProperty); }
            set { SetValue(MoveDownCommandProperty, value); }
        }

        public static readonly DependencyProperty MoveDownCommandProperty =
            DependencyProperty.Register(nameof(MoveDownCommand), typeof(ICommand), typeof(CollectionEditorControl));

    }
}
