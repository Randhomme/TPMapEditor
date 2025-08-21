using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class PolygonControl
    {
        public Path Path { get; }
        public Path OutlinePath { get; }
        public List<PolygonPointControl> PolygonPointControls { get; }
        public PathFigure PathFigure { get; }
        public WorldPolygon WorldPolygon { get; }
        public PolygonControl(Path path, Path outlinePath, PathFigure pathFigure, WorldPolygon worldPolygon)
        {
            Path = path;
            OutlinePath = outlinePath;
            PolygonPointControls = new List<PolygonPointControl>();
            PathFigure = pathFigure;
            WorldPolygon = worldPolygon;
        }
    }
}
