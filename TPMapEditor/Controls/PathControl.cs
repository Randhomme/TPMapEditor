using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using TPMapEditor.Data;

namespace TPMapEditor.Controls
{
    public class PathControl
    {
        public Path Path { get; }
        public Path OutlinePath { get; }
        public List<PathPointControl> PathPointControls { get; }
        public PathFigure PathFigure { get; }
        public WaypointPath WaypointPath { get; }
        public PathControl(Path path, Path outlinePath, PathFigure pathFigure, WaypointPath waypointPath)
        {
            Path = path;
            OutlinePath = outlinePath;
            PathPointControls = new List<PathPointControl>();
            PathFigure = pathFigure;
            WaypointPath = waypointPath;
        }
    }
}
