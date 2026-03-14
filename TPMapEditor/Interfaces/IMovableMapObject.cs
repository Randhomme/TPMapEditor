namespace TPMapEditor.Interfaces
{
    /// <summary>
    /// Movable map object in 3D
    /// </summary>
    public interface IMovableMapObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
