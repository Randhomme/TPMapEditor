using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TPMapEditor.Data
{
    /// <summary>
    /// A class representing a world object in the selection grid.
    /// </summary>
    public class WotGridItem
    {
        public static ObservableCollection<WotGridItem> WotTypes { get; } = new ObservableCollection<WotGridItem>();
        public BitmapImage? Image { get; set; }
        public string? Type { get; set; }

        public override string ToString()
        {
            return Type ?? "WorldObect";
        }
    }
}
