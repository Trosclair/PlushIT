using PlushIT.Utilities;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Line3D
    {
        private readonly Point3DCollection pts;
        private readonly Int32Collection indicies;
        private readonly Vector3D zeroVector;

        public IndexPoint3D StartPoint { get; set; }
        public IndexPoint3D Point1 { get; set; }
        public IndexPoint3D Point2 { get; set; }
        public IndexPoint3D EndPoint { get; set; }
        public IndexPoint3D Point3 { get; set; }
        public IndexPoint3D Point4 { get; set; }

        public Surface3D Surface1 { get; set; }
        public Surface3D Surface2 { get; set; }

        public Vector3D DisplayOffset { get; set; }
        public double Length { get; set; }

        private Line3D(IndexPoint3D startPoint, IndexPoint3D point1, IndexPoint3D point2, IndexPoint3D endPoint, 
            IndexPoint3D point3, IndexPoint3D point4, Surface3D surface1, Surface3D surface2)
        {
            StartPoint = startPoint;
            Point1 = point1;
            Point2 = point2;

            EndPoint = endPoint;
            Point3 = point3;
            Point4 = point4;

            Surface1 = surface1;
            Surface2 = surface2;

            if (Surface1.DisplayOffset != zeroVector && Surface2.DisplayOffset != zeroVector)
            {
                DisplayOffset = Surface1.DisplayOffset + Surface2.DisplayOffset;

                pts = [StartPoint.Point + DisplayOffset, Point1.Point + Surface1.DisplayOffset, Point2.Point + Surface1.DisplayOffset,
                    EndPoint.Point + DisplayOffset, Point3.Point + Surface2.DisplayOffset, Point4.Point + Surface2.DisplayOffset,

                    StartPoint.Point - DisplayOffset, Point1.Point - Surface1.DisplayOffset, Point2.Point - Surface1.DisplayOffset, 
                    EndPoint.Point - DisplayOffset, Point3.Point - Surface2.DisplayOffset, Point4.Point - Surface2.DisplayOffset];

                indicies = [0, 1, 2, 0, 2, 3, 3, 4, 5, 3, 5, 0,
                    6, 7, 8, 6, 8, 9, 9, 10, 11, 9, 11, 6];
            }
            else
            {
                pts = [StartPoint.Point, Point1.Point, Point2.Point, EndPoint.Point, Point3.Point, Point4.Point];

                indicies = [0, 1, 2, 0, 2, 3, 3, 4, 5, 3, 5, 0];
            }

            Length = ThirdDimensionalCalculations.DistanceBetweenPoints(StartPoint.Point, EndPoint.Point);
        }

        public void RenderLine(MeshGeometry3D geometryHover)
        {
            geometryHover.Positions = pts;
            geometryHover.TriangleIndices = indicies;
        }

        public static Line3D? TryCreateLine(Surface3D surface1, Surface3D surface2) 
        {
            Line3D? line = null;
            int edgeNumber = 0;

            if (surface1.Edge12 is null && surface2.GetSharedEdge(surface1.OuterTriangle.Point1, surface1.OuterTriangle.Point2) is (IndexPoint3D, IndexPoint3D, int) otherPts)
            {
                line = new(
                    surface1.OuterTriangle.Point1,
                    surface1.InnerTriangle.Point1,
                    surface1.InnerTriangle.Point2,
                    surface1.OuterTriangle.Point2,
                    otherPts.Item1,
                    otherPts.Item2,
                    surface1,
                    surface2);

                surface1.Edge12 = line;
                edgeNumber = otherPts.Item3;
            }
            else if (surface1.Edge23 is null && surface2.GetSharedEdge(surface1.OuterTriangle.Point2, surface1.OuterTriangle.Point3) is (IndexPoint3D, IndexPoint3D, int) otherPts1)
            {
                line = new(
                    surface1.OuterTriangle.Point2,
                    surface1.InnerTriangle.Point2,
                    surface1.InnerTriangle.Point3,
                    surface1.OuterTriangle.Point3,
                    otherPts1.Item1,
                    otherPts1.Item2,
                    surface1,
                    surface2);

                surface1.Edge23 = line;
                edgeNumber = otherPts1.Item3;
            }
            else if (surface1.Edge31 is null && surface2.GetSharedEdge(surface1.OuterTriangle.Point3, surface1.OuterTriangle.Point1) is (IndexPoint3D, IndexPoint3D, int) otherPts2)
            {
                line = new(
                    surface1.OuterTriangle.Point3,
                    surface1.InnerTriangle.Point3,
                    surface1.InnerTriangle.Point1,
                    surface1.OuterTriangle.Point1,
                    otherPts2.Item1,
                    otherPts2.Item2,
                    surface1,
                    surface2);

                surface1.Edge31 = line;
                edgeNumber = otherPts2.Item3;
            }

            if (edgeNumber == 1)
            {
                surface2.Edge12 = line;
            }
            else if (edgeNumber == 2)
            {
                surface2.Edge23 = line;
            }
            else if (edgeNumber == 3)
            {
                surface2.Edge31 = line;
            }

            return line;
        }
    }
}
