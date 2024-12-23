using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Triangle3D(NormalPoint3D p1, NormalPoint3D p2, NormalPoint3D p3)
    {
        public NormalPoint3D Point1 { get; set; } = p1;
        public NormalPoint3D Point2 { get; set; } = p2;
        public NormalPoint3D Point3 { get; set; } = p3;

        public double Point1Angle { get; set; }
        public double Point2Angle { get; set; }
        public double Point3Angle { get; set; }

        public Hex3D? Point1ToPoint2 { get; set; }
        public Hex3D? Point2ToPoint3 { get; set; }
        public Hex3D? Point3ToPoint1 { get; set; }

        public void SetLineSegments(Hex3D point1ToPoint2, Hex3D point2ToPoint3, Hex3D point3ToPoint1)
        {
            Point1ToPoint2 = point1ToPoint2;
            Point2ToPoint3 = point2ToPoint3;
            Point3ToPoint1 = point3ToPoint1;

            Point1.ConnectedTriangles.Add(this);
            Point2.ConnectedTriangles.Add(this);
            Point3.ConnectedTriangles.Add(this);

            Point1Angle = Math.Acos((Math.Pow(Point2ToPoint3.Length, 2) - Math.Pow(Point1ToPoint2.Length, 2) - Math.Pow(Point3ToPoint1.Length, 2)) /
                (-2 * Point3ToPoint1.Length * Point1ToPoint2.Length)) * (180D / Math.PI);

            Point3Angle = Math.Acos((Math.Pow(Point1ToPoint2.Length, 2) - Math.Pow(Point2ToPoint3.Length, 2) - Math.Pow(Point3ToPoint1.Length, 2)) /
                (-2 * Point2ToPoint3.Length * Point3ToPoint1.Length)) * (180D / Math.PI);

            Point2Angle = 180D - Point1Angle - Point3Angle; // Quick mafs! 

            Point1ToPoint2.Refresh();
            Point2ToPoint3.Refresh();
            Point3ToPoint1.Refresh();
        }
    }
}
