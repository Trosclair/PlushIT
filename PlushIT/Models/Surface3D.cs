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
        public NormalPoint3D OuterPoint1 { get; set; }
        public NormalPoint3D OuterPoint2 { get; set; }
        public NormalPoint3D OuterPoint3 { get; set; }

        public NormalPoint3D InnerPoint1 { get; set; }
        public NormalPoint3D InnerPoint2 { get; set; }
        public NormalPoint3D InnerPoint3 { get; set; }

        public double Thickness { get; set; } = 0.005;

        public Triangle3D(NormalPoint3D p1, NormalPoint3D p2, NormalPoint3D p3)
        {
            OuterPoint1 = p1;
            OuterPoint2 = p2;
            OuterPoint3 = p3;

            Point3D center = ThirdDimensionalCalculations.FindMidPoint(OuterPoint1.Point, OuterPoint2.Point, OuterPoint3.Point);

            Point3D midPoint12 = ThirdDimensionalCalculations.FindMidPoint(OuterPoint1.Point, OuterPoint2.Point);
            Point3D midPoint23 = ThirdDimensionalCalculations.FindMidPoint(OuterPoint2.Point, OuterPoint3.Point);
            Point3D midPoint13 = ThirdDimensionalCalculations.FindMidPoint(OuterPoint1.Point, OuterPoint3.Point);

            Vector3D midPoint12ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPoint12, center);
            double scale12 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint12ToCenter);
            scale12 = Math.Min(scale12, 1D);
            Vector3D midPoint12ToInnerTriangleEdge = midPoint12ToCenter * scale12;
            Point3D innerEdgePoint12 = Point3D.Add(midPoint12, midPoint12ToInnerTriangleEdge);

            Vector3D midPoint23ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPoint23, center);
            double scale23 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint23ToCenter);
            scale23 = Math.Min(scale23, 1D);
            Vector3D midPoint23ToInnerTriangleEdge = midPoint23ToCenter * scale23;
            Point3D innerEdgePoint23 = Point3D.Add(midPoint23, midPoint23ToInnerTriangleEdge);

            Vector3D midPoint13ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPoint13, center);
            double scale13 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint13ToCenter);
            scale13 = Math.Min(scale13, 1D);
            Vector3D midPoint13ToInnerTriangleEdge = midPoint13ToCenter * scale13;
            Point3D innerEdgePoint13 = Point3D.Add(midPoint13, midPoint13ToInnerTriangleEdge);

            /// Midpoint shenanigans...
            InnerPoint1 = new(new(
                innerEdgePoint12.X + innerEdgePoint13.X - innerEdgePoint23.X,
                innerEdgePoint12.Y + innerEdgePoint13.Y - innerEdgePoint23.Y,
                innerEdgePoint12.Z + innerEdgePoint13.Z - innerEdgePoint23.Z));

            InnerPoint2 = new(new(
                innerEdgePoint12.X - innerEdgePoint13.X + innerEdgePoint23.X,
                innerEdgePoint12.Y - innerEdgePoint13.Y + innerEdgePoint23.Y,
                innerEdgePoint12.Z - innerEdgePoint13.Z + innerEdgePoint23.Z));

            InnerPoint3 = new(new(
                innerEdgePoint13.X - innerEdgePoint12.X + innerEdgePoint23.X,
                innerEdgePoint13.Y - innerEdgePoint12.Y + innerEdgePoint23.Y,
                innerEdgePoint13.Z - innerEdgePoint12.Z + innerEdgePoint23.Z));
        }
    }
}
