using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class NormalPoint3D(Point3D point)
    {
        public int OuterPositionNumber { get; set; } = -1;
        public int InnerPositionNumber { get; set; } = -1;
        public Point3D Point { get; set; } = point;

    }
}
