using PlushIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{
    public static class ThirdDimensionalCalculations
    {
        public static Point3D FindMidPoint(params Point3D[] pts) => new(pts.Sum(x => x.X) / pts.Length, pts.Sum(x => x.Y) / pts.Length, pts.Sum(x => x.Z) / pts.Length);
        public static Vector3D VectorFromPoints(Point3D pt1, Point3D pt2) => new(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
        public static double DistanceBetweenPoints(Point3D pt1, Point3D pt2) => Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2) + Math.Pow(pt2.Z - pt1.Z, 2));
        public static double GetVectorMagnitude(Vector3D vec) => Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));
    }
}
