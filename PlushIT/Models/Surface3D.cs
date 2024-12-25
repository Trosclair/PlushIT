using HelixToolkit.Wpf;
using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Surface3D
    {
        public int SurfaceIndex { get; set; }

        public Triangle3D OuterTriangle { get; set; }
        public Triangle3D InnerTriangle { get; set; }
        public Triangle3D? InnerUpperTriangle { get; set; }
        public Triangle3D? InnerLowerTriangle { get; set; }

        public Line3D? Edge12 { get; set; }
        public Line3D? Edge23 { get; set; }
        public Line3D? Edge31 { get; set; }

        public Vector3D DisplayOffset { get; set; }
        public double Thickness { get; set; } = 0.005;

        public Surface3D(IndexPoint3D p1, IndexPoint3D p2, IndexPoint3D p3, int surfaceIndex)
        {
            SurfaceIndex = surfaceIndex;

            OuterTriangle = new(p1, p2, p3);

            Point3D center = ThirdDimensionalCalculations.FindMidPoint(OuterTriangle.Point1.Point, OuterTriangle.Point2.Point, OuterTriangle.Point3.Point);
                    
            Point3D midPointBetween1And2 = ThirdDimensionalCalculations.FindMidPoint(p1.Point, p2.Point);
            Point3D midPointBetween2And3 = ThirdDimensionalCalculations.FindMidPoint(p2.Point, p3.Point);
            Point3D midPointBetween3And1 = ThirdDimensionalCalculations.FindMidPoint(p1.Point, p3.Point);

            Vector3D midPoint12ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPointBetween1And2, center);
            double scale12 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint12ToCenter);
            scale12 = Math.Min(scale12, 1D);
            Vector3D midPoint12ToInnerTriangleEdge = midPoint12ToCenter * scale12;
            Point3D innerEdgePoint12 = Point3D.Add(midPointBetween1And2, midPoint12ToInnerTriangleEdge);

            Vector3D midPoint23ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPointBetween2And3, center);
            double scale23 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint23ToCenter);
            scale23 = Math.Min(scale23, 1D);
            Vector3D midPoint23ToInnerTriangleEdge = midPoint23ToCenter * scale23;
            Point3D innerEdgePoint23 = Point3D.Add(midPointBetween2And3, midPoint23ToInnerTriangleEdge);

            Vector3D midPoint13ToCenter = ThirdDimensionalCalculations.VectorFromPoints(midPointBetween3And1, center);
            double scale13 = Thickness / ThirdDimensionalCalculations.GetVectorMagnitude(midPoint13ToCenter);
            scale13 = Math.Min(scale13, 1D);
            Vector3D midPoint13ToInnerTriangleEdge = midPoint13ToCenter * scale13;
            Point3D innerEdgePoint13 = Point3D.Add(midPointBetween3And1, midPoint13ToInnerTriangleEdge);

            Vector3D normal = ThirdDimensionalCalculations.GetTriangleNormal(OuterTriangle.Point1.Point, OuterTriangle.Point2.Point, OuterTriangle.Point3.Point);

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

            OuterTriangle.Point1.ConnectedSurfaces.Add(this);
            OuterTriangle.Point2.ConnectedSurfaces.Add(this);
            OuterTriangle.Point3.ConnectedSurfaces.Add(this);

            InnerTriangle = new(new(point1), new(point2), new(point3));

            InnerTriangle.Point1.ConnectedSurfaces.Add(this);
            InnerTriangle.Point2.ConnectedSurfaces.Add(this);
            InnerTriangle.Point3.ConnectedSurfaces.Add(this);

            if (ThirdDimensionalCalculations.AreaOfTriangle(point1, point2, point3) > 1E-10) // zero wasn't filtering out some of the polygons with stupid low areas, still overkill though...
            {
                if (normal.X != 0D || normal.Y != 0D || normal.Z != 0D)
                {
                    DisplayOffset = ThirdDimensionalCalculations. GetUnitVector(normal) * .001;

                    InnerUpperTriangle = new(new(point1 + DisplayOffset), new(point2 + DisplayOffset), new(point3 + DisplayOffset));
                    InnerLowerTriangle = new(new(point1 - DisplayOffset), new(point2 - DisplayOffset), new(point3 - DisplayOffset));

                    InnerUpperTriangle.Point1.ConnectedSurfaces.Add(this);
                    InnerUpperTriangle.Point2.ConnectedSurfaces.Add(this);
                    InnerUpperTriangle.Point3.ConnectedSurfaces.Add(this);

                    InnerLowerTriangle.Point1.ConnectedSurfaces.Add(this);
                    InnerLowerTriangle.Point2.ConnectedSurfaces.Add(this);
                    InnerLowerTriangle.Point3.ConnectedSurfaces.Add(this);
                }
            }
        }

        public (IndexPoint3D, IndexPoint3D, int)? GetSharedEdge(IndexPoint3D pt1, IndexPoint3D pt2)
        {
            if (Edge12 == null &&
                ((pt1.OuterPositionNumber == OuterTriangle.Point1.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point2.OuterPositionNumber) ||
                (pt1.OuterPositionNumber == OuterTriangle.Point2.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point1.OuterPositionNumber)))
            {
                return (InnerTriangle.Point1, InnerTriangle.Point2, 1);
            }
            else if (Edge23 == null &&
                ((pt1.OuterPositionNumber == OuterTriangle.Point2.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point3.OuterPositionNumber) ||
                (pt1.OuterPositionNumber == OuterTriangle.Point3.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point2.OuterPositionNumber)))
            {
                return (InnerTriangle.Point2, InnerTriangle.Point3, 2);
            }
            else if (Edge31 == null &&
                ((pt1.OuterPositionNumber == OuterTriangle.Point3.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point1.OuterPositionNumber) ||
                (pt1.OuterPositionNumber == OuterTriangle.Point1.OuterPositionNumber && pt2.OuterPositionNumber == OuterTriangle.Point3.OuterPositionNumber)))
            {
                return (InnerTriangle.Point3, InnerTriangle.Point1, 3);
            }

            return null;
        }
    }
}
