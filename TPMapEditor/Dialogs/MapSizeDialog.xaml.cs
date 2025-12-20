using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using TPMapEditor.Data;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for MapSizeDialog.xaml
    /// </summary>
    public partial class MapSizeDialog : DialogWindow
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MaxWorldBuffer))]
        [NotifyPropertyChangedFor(nameof(SizeText))]
        private int size;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ZSizeText))]
        private int zSize;
        [ObservableProperty]
        private double worldBuffer;

        public double MaxWorldBuffer { get => Size / 2; }
        public string SizeText { get => $"{Size} x {Size}"; }
        public string ZSizeText { get => $"{ZSize} x {ZSize}"; }

        public MapSizeDialog(Window owner, string title, int size, int zSize, double worldBuffer) : base(owner, title)
        {
            this.size = size;
            this.zSize = zSize;
            this.worldBuffer = worldBuffer;
            InitializeComponent();
            DataContext = this;
        }

        partial void OnSizeChanged(int value)
        {
            if (WorldBuffer > MaxWorldBuffer)
                WorldBuffer = MaxWorldBuffer;
        }
    }
}
