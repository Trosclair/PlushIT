using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class NormalPoint3D(Point3D point)
    {
        public Point3D Point { get; set; } = point;
        public Vector3D Normal { get; set; }

        public List<Triangle3D> ConnectedTriangles { get; set; } = [];

    }
}
