using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for CollectionEditorDialog.xaml
    /// </summary>
    public partial class CollectionEditorDialog : DialogWindow
    {
        public IEnumerable<object> ItemsSource { get; }
        public Func<object> Factory { get; }
        public bool GridOnlyMode { get; }
        public CollectionEditorDialog(Window owner, IEnumerable<object> itemSource, Func<object> factory, bool gridOnlyMode = false) : base(owner)
        {
            this.ItemsSource = itemSource;
            this.Factory = factory;
            this.GridOnlyMode = gridOnlyMode;
            InitializeComponent();
        }
    }
}
