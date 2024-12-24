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
        public int SurfaceIndex { get; set; }

        public Triangle3D OuterTriangle { get; set; }
        public Triangle3D? InnerTriangle { get; set; }

        public Edge3D? Edge12 { get; set; }
        public Edge3D? Edge23 { get; set; }
        public Edge3D? Edge31 { get; set; }

        public double Thickness { get; set; } = 0.005;

        public Surface3D(IndexPoint3D p1, IndexPoint3D p2, IndexPoint3D p3, int surfaceIndex)
        {
            SurfaceIndex = surfaceIndex;

            OuterTriangle = new(p1, p2, p3);

            Vector3D midPoint12ToCenter = ThirdDimensionalCalculations.VectorFromPoints(OuterTriangle.MidPointBetween1And2, OuterTriangle.Center);
            double scale12 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint12ToCenter);
            scale12 = Math.Min(scale12, 1D);
            Vector3D midPoint12ToInnerTriangleEdge = midPoint12ToCenter * scale12;
            Point3D innerEdgePoint12 = Point3D.Add(OuterTriangle.MidPointBetween1And2, midPoint12ToInnerTriangleEdge);

            Vector3D midPoint23ToCenter = ThirdDimensionalCalculations.VectorFromPoints(OuterTriangle.MidPointBetween2And3, OuterTriangle.Center);
            double scale23 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint23ToCenter);
            scale23 = Math.Min(scale23, 1D);
            Vector3D midPoint23ToInnerTriangleEdge = midPoint23ToCenter * scale23;
            Point3D innerEdgePoint23 = Point3D.Add(OuterTriangle.MidPointBetween2And3, midPoint23ToInnerTriangleEdge);

            Vector3D midPoint13ToCenter = ThirdDimensionalCalculations.VectorFromPoints(OuterTriangle.MidPointBetween3And1, OuterTriangle.Center);
            double scale13 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint13ToCenter);
            scale13 = Math.Min(scale13, 1D);
            Vector3D midPoint13ToInnerTriangleEdge = midPoint13ToCenter * scale13;
            Point3D innerEdgePoint13 = Point3D.Add(OuterTriangle.MidPointBetween3And1, midPoint13ToInnerTriangleEdge);

            /// Midpoint shenanigans...
            Point3D point1 = new(
                innerEdgePoint12.X + innerEdgePoint13.X - innerEdgePoint23.X,
                innerEdgePoint12.Y + innerEdgePoint13.Y - innerEdgePoint23.Y,
                innerEdgePoint12.Z + innerEdgePoint13.Z - innerEdgePoint23.Z);

            Point3D point2 = new(
                innerEdgePoint12.X - innerEdgePoint13.X + innerEdgePoint23.X,
                innerEdgePoint12.Y - innerEdgePoint13.Y + innerEdgePoint23.Y,
                innerEdgePoint12.Z - innerEdgePoint13.Z + innerEdgePoint23.Z);

            Point3D point3 = new(
                innerEdgePoint13.X - innerEdgePoint12.X + innerEdgePoint23.X,
                innerEdgePoint13.Y - innerEdgePoint12.Y + innerEdgePoint23.Y,
                innerEdgePoint13.Z - innerEdgePoint12.Z + innerEdgePoint23.Z);

            if (ThirdDimensionalCalculations.AreaOfTriangle(point1, point2, point3) != 0D)
            {
                InnerTriangle = new(new(point1), new(point2), new(point3));

                OuterTriangle.Point1.ConnectedTriangles.Add(this);
                OuterTriangle.Point2.ConnectedTriangles.Add(this);
                OuterTriangle.Point3.ConnectedTriangles.Add(this);

                InnerTriangle.Point1.ConnectedTriangles.Add(this);
                InnerTriangle.Point2.ConnectedTriangles.Add(this);
                InnerTriangle.Point3.ConnectedTriangles.Add(this);

                Edge12 = new(OuterTriangle.Point1, InnerTriangle.Point1, InnerTriangle.Point2, OuterTriangle.Point2);
                Edge23 = new(OuterTriangle.Point2, InnerTriangle.Point2, InnerTriangle.Point3, OuterTriangle.Point3);
                Edge31 = new(OuterTriangle.Point3, InnerTriangle.Point3, InnerTriangle.Point1, OuterTriangle.Point1);
            }
        }

        public Edge3D? FindClosestEdge(Point3D touchPoint)
        {
            double distTo12 = ThirdDimensionalCalculations.DistanceFromPoint1ToLine23(touchPoint, OuterTriangle.Point1.Point, OuterTriangle.Point2.Point);
            double distTo23 = ThirdDimensionalCalculations.DistanceFromPoint1ToLine23(touchPoint, OuterTriangle.Point2.Point, OuterTriangle.Point3.Point);
            double distTo31 = ThirdDimensionalCalculations.DistanceFromPoint1ToLine23(touchPoint, OuterTriangle.Point3.Point, OuterTriangle.Point1.Point);

            if (distTo12 < distTo23 && distTo12 < distTo31)
            {
                return Edge12;
            }
            else if (distTo23 < distTo12 && distTo23 < distTo31)
            {
                return Edge23;
            }
            else
            {
                return Edge31;
            }
        }

        public Edge3D? GetSharedEdge(Edge3D edge)
        {
            if (edge.IsEdgeShared(Edge12))
            {
                return Edge12;
            }
            else if (edge.IsEdgeShared(Edge23))
            {
                return Edge23;
            }
            else if (edge.IsEdgeShared(Edge31))
            {
                return Edge31;
            }

            return null;
        }

        public bool IsSuppliedPointAVertice(Point3D point)
        {
            return OuterTriangle.IsPointAVertice(point) || (InnerTriangle is not null && InnerTriangle.IsPointAVertice(point));
        }
    }
}
