using HelixToolkit.Wpf;
using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Surface3D
    {
        public Triangle3D OuterTriangle { get; set; }
        public Triangle3D? InnerTriangle { get; set; }
        public IndexPoint3D ScramblePoint { get; set; }

        public double Thickness { get; set; } = 0.005;

        public Surface3D(IndexPoint3D p1, IndexPoint3D p2, IndexPoint3D p3)
        {
            OuterTriangle = new(p1, p2, p3);

            Point3D center = ThirdDimensionalCalculations.FindMidPoint(OuterTriangle.Point1.Point, OuterTriangle.Point2.Point, OuterTriangle.Point3.Point);

            Point3D midPoint12 = ThirdDimensionalCalculations.FindMidPoint(OuterTriangle.Point1.Point, OuterTriangle.Point2.Point);
            Point3D midPoint23 = ThirdDimensionalCalculations.FindMidPoint(OuterTriangle.Point2.Point, OuterTriangle.Point3.Point);
            Point3D midPoint13 = ThirdDimensionalCalculations.FindMidPoint(OuterTriangle.Point1.Point, OuterTriangle.Point3.Point);

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
            IndexPoint3D point1 = new(new(
                innerEdgePoint12.X + innerEdgePoint13.X - innerEdgePoint23.X,
                innerEdgePoint12.Y + innerEdgePoint13.Y - innerEdgePoint23.Y,
                innerEdgePoint12.Z + innerEdgePoint13.Z - innerEdgePoint23.Z));

            IndexPoint3D point2 = new(new(
                innerEdgePoint12.X - innerEdgePoint13.X + innerEdgePoint23.X,
                innerEdgePoint12.Y - innerEdgePoint13.Y + innerEdgePoint23.Y,
                innerEdgePoint12.Z - innerEdgePoint13.Z + innerEdgePoint23.Z));

            IndexPoint3D point3 = new(new(
                innerEdgePoint13.X - innerEdgePoint12.X + innerEdgePoint23.X,
                innerEdgePoint13.Y - innerEdgePoint12.Y + innerEdgePoint23.Y,
                innerEdgePoint13.Z - innerEdgePoint12.Z + innerEdgePoint23.Z));

            if (ThirdDimensionalCalculations.AreaOfTriangle(point1.Point, point2.Point, point3.Point) != 0D)
            {
                InnerTriangle = new(point1, point2, point3);
            }

            ScramblePoint = new(center);

            OuterTriangle.Point1.ConnectedTriangles.Add(this);
            OuterTriangle.Point2.ConnectedTriangles.Add(this);
            OuterTriangle.Point3.ConnectedTriangles.Add(this);
        }
    }
}
