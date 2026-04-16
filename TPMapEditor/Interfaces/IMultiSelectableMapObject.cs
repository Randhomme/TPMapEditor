namespace TPMapEditor.Interfaces
{
    public interface IMultiSelectableMapObject
    {
        public bool UseUpdateCommands { get; set; }
        public int Count { get; }
    }
}
