using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{
    public static class ThirdDimensionalCalculations
    {
        public static double DistanceFromPoint1ToLine23(Point3D pt1, Point3D pt2, Point3D pt3)
        {
            Vector3D d = (pt3 - pt2) / DistanceBetweenPoints(pt3, pt2);
            Vector3D vec12 = pt1 - pt2;
            double t = Vector3D.DotProduct(vec12, d);
            Point3D p = pt2 + (t * d);
            return DistanceBetweenPoints(p, pt1);
        }

        public static double AreaOfTriangle(Point3D pt1, Point3D pt2, Point3D pt3)
        {
            double a = DistanceBetweenPoints(pt1, pt2);
            double b = DistanceBetweenPoints(pt2, pt3);
            double c = DistanceBetweenPoints(pt3, pt1);
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        public static Point3D FindMidPoint(params Point3D[] pts) => new(pts.Sum(x => x.X) / pts.Length, pts.Sum(x => x.Y) / pts.Length, pts.Sum(x => x.Z) / pts.Length);
        public static Vector3D VectorFromPoints(Point3D pt1, Point3D pt2) => new(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
        public static double DistanceBetweenPoints(Point3D pt1, Point3D pt2) => Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2) + Math.Pow(pt2.Z - pt1.Z, 2));
        public static double GetVectorMagnitude(Vector3D vec) => Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));
    }
}
