using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class IndexPoint3D(Point3D point, int innerPositionNumber = -1, int outerPositionNumber = -1)
    {
        public int InnerPositionNumber { get; set; } = innerPositionNumber;
        public int OuterPositionNumber { get; set; } = outerPositionNumber;
        public Point3D Point { get; set; } = point;

        public List<Surface3D> ConnectedSurfaces { get; set; } = [];
        public List<Line3D> ConnectedLines { get; set; } = [];
    }
}
