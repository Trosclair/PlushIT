using PlushIT.Utilities;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Line3D
    {
        private readonly Point3D[] pts, ptsHover;
        private readonly int[] unitIndicies;
        private static readonly Vector3D zeroVector = new(0, 0, 0);

        public IndexPoint3D StartPoint { get; set; }
        public IndexPoint3D Point1 { get; set; }
        public IndexPoint3D Point2 { get; set; }
        public IndexPoint3D EndPoint { get; set; }
        public IndexPoint3D Point3 { get; set; }
        public IndexPoint3D Point4 { get; set; }

        public Surface3D Surface1 { get; set; }
        public Surface3D Surface2 { get; set; }

        public int LineIndex { get; set; }

        private Line3D(int lineIndex, IndexPoint3D startPoint, IndexPoint3D point1, IndexPoint3D point2, IndexPoint3D endPoint, 
            IndexPoint3D point3, IndexPoint3D point4, Surface3D surface1, Surface3D surface2)
        {
            LineIndex = lineIndex;

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

                pts = [StartPoint.Point, Point1.Point + Surface1.DisplayOffset, Point2.Point + Surface1.DisplayOffset,
                    EndPoint.Point, Point3.Point + Surface2.DisplayOffset, Point4.Point + Surface2.DisplayOffset,

                    StartPoint.Point, Point1.Point - Surface1.DisplayOffset, Point2.Point - Surface1.DisplayOffset, 
                    EndPoint.Point, Point3.Point - Surface2.DisplayOffset, Point4.Point - Surface2.DisplayOffset];

                ptsHover = 
                    [StartPoint.Point, Point1.Point + (Surface1.DisplayOffset * 2), Point2.Point + (Surface1.DisplayOffset * 2),
                    EndPoint.Point, Point3.Point + (Surface2.DisplayOffset * 2), Point4.Point + (Surface2.DisplayOffset * 2),

                    StartPoint.Point, Point1.Point - (Surface1.DisplayOffset * 2), Point2.Point - (Surface1.DisplayOffset * 2),
                    EndPoint.Point, Point3.Point - (Surface2.DisplayOffset * 2), Point4.Point - (Surface2.DisplayOffset * 2)];

                unitIndicies = [0, 1, 2, 0, 2, 3, 3, 4, 5, 3, 5, 0, 6, 7, 8, 6, 8, 9, 9, 10, 11, 9, 11, 6];
            }
            else
            {
                pts = ptsHover = [StartPoint.Point, Point1.Point, Point2.Point, EndPoint.Point, Point3.Point, Point4.Point];

                unitIndicies = [0, 1, 2, 0, 2, 3, 3, 4, 5, 3, 5, 0];

            }
        }

        public void RenderHover(MeshGeometry3D geometry)
        {
            geometry.Positions.Clear();
            geometry.TriangleIndices.Clear();

            foreach (Point3D pt in ptsHover)
            {
                geometry.Positions.Add(pt);
            }

            foreach (int i in unitIndicies)
            {
                geometry.TriangleIndices.Add(i);
            }
        }

        public void Render(MeshGeometry3D geometry)
        {
            foreach (int i in unitIndicies.Select(x => x + geometry.Positions.Count))
            {
                geometry.TriangleIndices.Add(i);
            }

            foreach (Point3D pt in pts)
            {
                geometry.Positions.Add(pt);
            }
        }

        public void UnRender(MeshGeometry3D geometry)
        {
            int lowestTriangleIndicie = 0;
            for (int i = 0; i < geometry.Positions.Count - pts.Length + 1; i++)
            {
                if (geometry.Positions[i] == pts[0])
                {
                    bool isMatch = true;
                    for (int j = 1; j < pts.Length; j++)
                    {
                        if (geometry.Positions[j + i] != pts[j])
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        lowestTriangleIndicie = i;
                        for (int j = pts.Length + i - 1; j >= i; j--)
                        {
                            geometry.Positions.RemoveAt(j);
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < geometry.TriangleIndices.Count; i++)
            {
                if (geometry.TriangleIndices[i] == lowestTriangleIndicie)
                {
                    for (int j = 0; j < unitIndicies.Length; j++)
                    {
                        geometry.TriangleIndices.RemoveAt(i);
                    }

                    for (int j = i; j < geometry.TriangleIndices.Count; j++)
                    {
                        geometry.TriangleIndices[j] -= pts.Length;
                    }

                    break;
                }
            }
        }

        public static Line3D? TryCreateLine(Surface3D surface1, Surface3D surface2, int lineIndex) 
        {
            Line3D? line = null;
            int edgeNumber = 0;

            if (surface1.Edge12 is null && surface2.GetSharedEdge(surface1.OuterTriangle.Point1, surface1.OuterTriangle.Point2) is (IndexPoint3D, IndexPoint3D, int) otherPts)
            {
                line = new(lineIndex,
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
                line = new(lineIndex,
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
                line = new(lineIndex,
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
