using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{
    public static class ThirdDimensionalCalculations
    {
        public static Vector3D GetUnitVector(Vector3D vect)
        {
            return vect / GetVectorMagnitude(vect);
        }

        public static Vector3D GetTriangleNormal(Point3D pt1, Point3D pt2, Point3D pt3)
        {
            Vector3D u = pt2 - pt1;
            Vector3D v = pt3 - pt1;

            double x = (u.Y * v.Z) - (u.Z * v.Y);
            double y = (u.Z * v.X) - (u.X * v.Z);
            double z = (u.X * v.Y) - (u.Y * v.X);

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Distance from pt1 to line segment pt2/pt3.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt3"></param>
        /// <returns></returns>
        public static double DistanceFromPointToLineSegment(Point3D pt1, Point3D pt2, Point3D pt3)
        {
            Vector3D d = (pt3 - pt2) / DistanceBetweenPoints(pt3, pt2);
            Vector3D vec12 = pt1 - pt2;
            double t = Vector3D.DotProduct(vec12, d);
            Point3D p = pt2 + (t * d);

            double magP = GetVectorMagnitude(p);
            double magPt2 = GetVectorMagnitude(pt2);
            double magPt3 = GetVectorMagnitude(pt3);

            if ((magP < magPt2 && magP < magPt3) || // Check whether P falls in the bounds of Pt2 and Pt3... If not
                (magP > magPt2 && magP > magPt3))   // give up and find the closer of Pt2 and Pt3.
            {
                return Math.Min(DistanceBetweenPoints(pt1, pt2), DistanceBetweenPoints(pt1, pt3));
            }
            return DistanceBetweenPoints(p, pt1);
        }

        /// <summary>
        /// Only use this method if you are 100% sure pt1 is between pt2 and pt3. 
        /// or if you need the distance from a point to a line and not a line segment
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt3"></param>
        /// <returns></returns>
        public static double DistanceFromPointToLine(Point3D pt1, Point3D pt2, Point3D pt3)
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
        public static Vector3D VectorFromPoints(Point3D pt1, Point3D pt2) => pt2 - pt1;
        public static double DistanceBetweenPoints(Point3D pt1, Point3D pt2) => Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2) + Math.Pow(pt2.Z - pt1.Z, 2));
        public static double GetVectorMagnitude(Vector3D vec) => Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));
        public static double GetVectorMagnitude(Point3D vec) => Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));
    }
}
