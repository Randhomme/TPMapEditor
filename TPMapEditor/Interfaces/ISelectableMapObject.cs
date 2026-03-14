namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Represents a selectable object on the map.
    /// </summary>
    public interface ISelectableMapObject : ICopiableMapObject
    {
        public bool IsSelected { get; set; }
        public bool IsLastSelected { get; set; }
        public bool IsShownOnUi { get; set; }
        public int ZIndex { get; set; }
        //public ISelectableMapObject Copy();
    }
}
