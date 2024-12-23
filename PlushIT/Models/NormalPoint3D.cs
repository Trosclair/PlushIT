using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class NormalPoint3D(Point3D point, int pointNumber = -1)
    {
        public int PointNumber { get; set; } = pointNumber;
        public Point3D Point { get; set; } = point;
        public Vector3D Normal { get; set; }

        public List<Triangle3D> ConnectedTriangles { get; set; } = [];
        public Dictionary<int, NormalPoint3D> AdjacentPoints { get; set; } = [];

    }
}
