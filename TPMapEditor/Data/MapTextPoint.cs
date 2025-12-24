using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class MapTextPoint : NamedElement
    {
        [ObservableProperty]
        private string realText;
        [ObservableProperty]
        private double x, y, z;
        [ObservableProperty]
        private bool visible, isSelected, isLastSelected, isShownOnUi = true;

        public string DisplayedText
        {
            get
            {
                StringDictionnary.MapTextItems.TryGetValue(RealText, out string displayedText);
                return displayedText;
            }
        }

        public MapTextPoint(WorldMap map, string name, string realText, double x = 0, double y = 0, double z = 0, bool visible = true) : base(map, name)
        {
            this.realText = realText;
            this.x = x;
            this.y = y;
            this.z = z;
            this.visible = visible;
        }

        partial void OnRealTextChanged(string value)
        {
            OnPropertyChanged(nameof(DisplayedText));
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in Map.MapTextPoints)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
