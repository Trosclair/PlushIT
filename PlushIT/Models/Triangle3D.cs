using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Triangle3D
    {
        public IndexPoint3D Point1 { get; set; }
        public IndexPoint3D Point2 { get; set; }
        public IndexPoint3D Point3 { get; set; }

        public Point3D Center { get; set; }

        public Point3D MidPointBetween1And2 { get; set; }
        public Point3D MidPointBetween2And3 { get; set; }
        public Point3D MidPointBetween3And1 { get; set; }

        public Triangle3D(IndexPoint3D pt1, IndexPoint3D pt2, IndexPoint3D pt3)
        {
            Point1 = pt1;
            Point2 = pt2;
            Point3 = pt3;

            Center = ThirdDimensionalCalculations.FindMidPoint(Point1.Point, Point2.Point, Point3.Point);

            MidPointBetween1And2 = ThirdDimensionalCalculations.FindMidPoint(Point1.Point, Point2.Point);
            MidPointBetween2And3 = ThirdDimensionalCalculations.FindMidPoint(Point2.Point, Point3.Point);
            MidPointBetween3And1 = ThirdDimensionalCalculations.FindMidPoint(Point1.Point, Point3.Point);
        }

        public bool IsPointAVertice(Point3D point)
        {
            return Point1.Point == point ||
                Point2.Point == point ||
                Point3.Point == point;
        }
    }
}
